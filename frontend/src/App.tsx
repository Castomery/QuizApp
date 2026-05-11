import { Routes, Route, Navigate } from 'react-router'
import { LobbyPage } from './pages/LobbyPage'
import { WaitingPage } from './pages/WaitingPage'
import { GamePage } from './pages/GamePage'
import { ResultsPage } from './pages/ResultsPage'
import { GeneratingPage } from './pages/GeneratingPage'
import { useGameRoute } from './hooks/useGameRoute'

export default function App() {
  useGameRoute() // автоматично редіректить по фазі

  return (
    <Routes>
      <Route path="/" element={<LobbyPage />} />
      <Route path="/waiting" element={<WaitingPage />} />
      <Route path="/generating" element={<GeneratingPage />} />
      <Route path="/game" element={<GamePage />} />
      <Route path="/results" element={<ResultsPage />} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}