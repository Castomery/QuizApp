import { useState } from 'react'
import { useGame } from '../hooks/useGame'
import { Button } from '../components/ui/Button'
import { Input } from '../components/ui/Input'

type Mode = 'home' | 'create' | 'join'

export const LobbyPage = () => {
  const { createRoom, joinRoom } = useGame()

  const [mode, setMode] = useState<Mode>('home')
  const [playerName, setPlayerName] = useState('')
  const [topic, setTopic] = useState('')
  const [difficulty, setDifficulty] = useState(2)
  const [roomCode, setRoomCode] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleCreate = async () => {
  if (!playerName.trim()) { setError('Введи своє імʼя'); return }
  if (!topic.trim()) { setError('Введи тему вікторини'); return }

  setError('')
  setLoading(true)
  try {
    console.log('1. Створюємо кімнату...')
    const code = await createRoom(topic, difficulty)
    console.log('2. Кімната створена:', code)
    await joinRoom(code, playerName)
    console.log('3. Приєднались успішно')
  } catch (e) {
    console.error('Помилка:', e)
    setError('Не вдалось створити кімнату')
  } finally {
    setLoading(false)
  }
}

  const handleJoin = async () => {
    if (!playerName.trim()) { setError('Введи своє імʼя'); return }
    if (!roomCode.trim()) { setError('Введи код кімнати'); return }

    setError('')
    setLoading(true)
    try {
      await joinRoom(roomCode.toUpperCase(), playerName)
    } catch {
      setError('Кімнату не знайдено або гра вже почалась')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-950 via-purple-950 to-slate-900 flex items-center justify-center p-4">
      <div className="w-full max-w-md">

        {/* Заголовок */}
        <div className="text-center mb-10">
          <h1 className="text-5xl font-bold text-white mb-2">🧠 QuizApp</h1>
          <p className="text-white/50 text-sm">Вікторина з AI-ведучим</p>
        </div>

        {/* Home */}
        {mode === 'home' && (
          <div className="flex flex-col gap-3">
            <Button
              label="Створити кімнату"
              onClick={() => setMode('create')}
              fullWidth
            />
            <Button
              label="Приєднатись"
              onClick={() => setMode('join')}
              variant="secondary"
              fullWidth
            />
          </div>
        )}

        {/* Create */}
        {mode === 'create' && (
          <div className="flex flex-col gap-4">
            <Input
              label="Твоє імʼя"
              value={playerName}
              onChange={setPlayerName}
              placeholder="Андрій"
              maxLength={20}
              autoFocus
            />
            <Input
              label="Тема вікторини"
              value={topic}
              onChange={setTopic}
              placeholder="Програмування, Географія, Кіно..."
              maxLength={50}
            />
            <div className="flex flex-col gap-1">
              <label className="text-sm text-white/60 font-medium">
                Складність — {difficulty}/5
              </label>
              <input
                type="range"
                min={1}
                max={5}
                value={difficulty}
                onChange={(e) => setDifficulty(Number(e.target.value))}
                className="accent-indigo-500"
              />
              <div className="flex justify-between text-xs text-white/30">
                <span>Легко</span>
                <span>Важко</span>
              </div>
            </div>

            {error && <p className="text-red-400 text-sm">{error}</p>}

            <Button
              label="Створити"
              onClick={handleCreate}
              loading={loading}
              fullWidth
            />
            <Button
              label="Назад"
              onClick={() => { setMode('home'); setError('') }}
              variant="secondary"
              fullWidth
            />
          </div>
        )}

        {/* Join */}
        {mode === 'join' && (
          <div className="flex flex-col gap-4">
            <Input
              label="Твоє імʼя"
              value={playerName}
              onChange={setPlayerName}
              placeholder="Андрій"
              maxLength={20}
              autoFocus
            />
            <Input
              label="Код кімнати"
              value={roomCode}
              onChange={(v) => setRoomCode(v.toUpperCase())}
              placeholder="482910"
              maxLength={6}
            />

            {error && <p className="text-red-400 text-sm">{error}</p>}

            <Button
              label="Приєднатись"
              onClick={handleJoin}
              loading={loading}
              fullWidth
            />
            <Button
              label="Назад"
              onClick={() => { setMode('home'); setError('') }}
              variant="secondary"
              fullWidth
            />
          </div>
        )}

      </div>
    </div>
  )
}