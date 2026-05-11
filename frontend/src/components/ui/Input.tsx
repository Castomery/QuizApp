interface Props {
  value: string
  onChange: (value: string) => void
  placeholder?: string
  label?: string
  maxLength?: number
  autoFocus?: boolean
}

export const Input = ({ value, onChange, placeholder, label, maxLength, autoFocus }: Props) => (
  <div className="flex flex-col gap-1">
    {label && (
      <label className="text-sm text-white/60 font-medium">{label}</label>
    )}
    <input
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      maxLength={maxLength}
      autoFocus={autoFocus}
      className="bg-white/10 border border-white/20 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:outline-none focus:border-indigo-400 transition-colors"
    />
  </div>
)