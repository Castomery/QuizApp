export interface Player {
  username: string
  avatarColor: string
  score: number
  rank?: number
  hasAnswered?: boolean
}

export interface Question {
  text: string
  options: string[]
  timeoutSec: number
}

export interface RoomState {
  roomCode: string
  topic: string
  difficulty: number
  players: Player[]
}

export interface JoinRoomResponse {
  playerSessionId: string
  playerId: string
  token: string
  username: string
  avatarColor: string
}

export interface CreateRoomResponse {
  roomCode: string
  id: string
}

export interface AnswerResult {
  isCorrect: boolean
  pointsEarned: number
  newScore: number
  streak: number
}

export interface RoundEndedData {
  round: number
  correctAnswerIndex: number
  explanation: string
  aiComment: string
}

export interface GameFinishedData {
  leaderboard: Player[]
  mvp: string
  aiSummary: string
}

export interface RoomStateData {
  roomCode: string
  topic: string
  difficulty: number
  players: Player[]
}

export type GamePhase = 'lobby' | 'waiting' | 'generating' | 'playing' | 'roundEnd' | 'results'