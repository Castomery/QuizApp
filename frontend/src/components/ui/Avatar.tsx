interface Props {
  username: string
  color: string
  size?: 'sm' | 'md' | 'lg'
}

export const Avatar = ({ username, color, size = 'md' }: Props) => {
  const sizes = {
    sm: 'w-8 h-8 text-xs',
    md: 'w-10 h-10 text-sm',
    lg: 'w-14 h-14 text-lg',
  }

  return (
    <div
      className={`${sizes[size]} rounded-full flex items-center justify-center font-bold text-white flex-shrink-0`}
      style={{ backgroundColor: color }}
    >
      {username.charAt(0).toUpperCase()}
    </div>
  )
}