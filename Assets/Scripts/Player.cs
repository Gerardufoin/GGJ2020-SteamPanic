using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CustomPhysics
{
    public enum e_Upgrades
    {
        SPEED,
        JUMP,
        REPAIR,
        STEAM_GEN
    }

    public float Speed = 8f;
    public float JumpPower = 15f;
    [Range(0, 2)]
    public int PanicLevel = 0;
    public float FixLevel = 10f;

    public List<AudioClip> _repairSfx = new List<AudioClip>();

    private SpriteRenderer _renderer;
    private Animator _animator;
    private AudioSource _audio;
    private BGMManager _bgm;
    private SteamMachine _machine;
    private bool _jumpReleased;
    private bool _repairing;

    private Pipe _pipe = null;
    private UpgradeStation _station = null;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _machine = FindObjectOfType<SteamMachine>();
        _bgm = FindObjectOfType<BGMManager>();
    }

    private void Repair()
    {
        if (_pipe != null)
        {
            _repairing = true;
            _animator.SetBool("Repair", true);
        }
        else
        {
            EndRepair();
        }
    }

    private void EndRepair()
    {
        _repairing = false;
        _animator.SetBool("Repair", false);
        _audio.Stop();
    }

    private void BuyUpgrade()
    {
        if (_station != null)
        {
            _station.Buy();
        }
    }

    protected new void Update()
    {
        base.Update();

        if (!_machine.Running)
        {
            return;
        }

        if (IsGrounded && Input.GetButtonDown("Repair"))
        {
            Repair();
            BuyUpgrade();
        }
        if (_repairing)
        {
            if (!_audio.isPlaying)
            {
                _audio.PlayOneShot(_repairSfx[Random.Range(0, _repairSfx.Count)]);
            }
            if (_pipe == null || Input.GetButtonUp("Repair"))
            {
                EndRepair();
            }
            else
            {
                _pipe.Fix(FixLevel * Time.deltaTime);
            }
        }
        int prev = PanicLevel;
        PanicLevel = 2 - Mathf.RoundToInt(_machine.LifeRatio * 2);
        _bgm.SetPitch(1f + PanicLevel * 0.15f);
        if (prev < 2 && PanicLevel >= 2)
        {
            _animator.SetTrigger("Panic");
        }
        if (prev >= 2 && PanicLevel < 2)
        {
            _animator.SetTrigger("StopPanic");
        }
        _animator.SetInteger("PanicLevel", PanicLevel);
    }

    protected override void ComputeVelocity()
    {
        if (IsGrounded)
        {
            _jumpReleased = false;
        }

        if (!_repairing)
        {
            float movement = Input.GetAxis("Horizontal");
            m_targetVelocity = new Vector2(movement * Speed, 0f);

            if (IsGrounded && Input.GetButtonDown("Jump"))
            {
                m_velocity.y = JumpPower;
            }
            else if (!_jumpReleased && Input.GetButtonUp("Jump"))
            {
                _jumpReleased = true;
                if (m_velocity.y > 0)
                {
                    m_velocity.y /= 2;
                }
            }

            bool flip = (_renderer.flipX ? (movement > 0.01f) : (movement < 0.01f));
            if (flip)
            {
                _renderer.flipX = !_renderer.flipX;
            }
            _animator.SetFloat("Velocity", Mathf.Abs(m_velocity.x) / Speed);
        }

        _animator.SetBool("Grounded", IsGrounded);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Pipe")
        {
            _pipe = other.GetComponent<Pipe>();
        }
        if (other.tag == "UpgradeStation")
        {
            _station = other.GetComponent<UpgradeStation>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_pipe != null && collision.gameObject == _pipe.gameObject)
        {
            _pipe = null;
        }
        if (_station != null && collision.gameObject == _station.gameObject)
        {
            _station = null;
        }
    }

    public void UpgradeSpeed()
    {
        Speed += 1f;
    }

    public void UpgradeJump()
    {
        JumpPower += 1f;
    }

    public void UpgradeRepair()
    {
        FixLevel += 2.5f;
    }
}
