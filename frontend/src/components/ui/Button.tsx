interface Props {
  label: string
  onClick?: () => void
  disabled?: boolean
  loading?: boolean
  variant?: 'primary' | 'danger' | 'secondary'
  type?: 'button' | 'submit'
  fullWidth?: boolean
}

export const Button = ({
  label,
  onClick,
  disabled,
  loading,
  variant = 'primary',
  type = 'button',
  fullWidth,
}: Props) => {
  const base = 'px-6 py-3 rounded-xl font-semibold transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed'

  const variants = {
    primary: 'bg-indigo-600 hover:bg-indigo-500 text-white',
    danger: 'bg-red-600 hover:bg-red-500 text-white',
    secondary: 'bg-white/10 hover:bg-white/20 text-white border border-white/20',
  }

  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled || loading}
      className={`${base} ${variants[variant]} ${fullWidth ? 'w-full' : ''}`}
    >
      {loading ? 'Завантаження...' : label}
    </button>
  )
}