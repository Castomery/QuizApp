import { useGameStore } from '../store/gameStore'
import { useSignalR } from './useSignalR'
import { createRoom, joinRoom } from '../api/roomsApi'

export const useGame = () => {
  const store = useGameStore()
  const { connect, disconnect, invoke } = useSignalR()

  const handleCreateRoom = async (topic: string, difficulty: number) => {
    const room = await createRoom(topic, difficulty)
    store.setRoomCode(room.roomCode)
    return room.roomCode
  }

  const handleJoinRoom = async (roomCode: string, playerName: string) => {

    const result = await joinRoom(roomCode, playerName)

    store.setPlayerInfo(
      result.username,
      result.avatarColor,
      result.token,
      result.playerId
    )
    store.setRoomCode(roomCode)

     const conn = await connect()

    await invoke(conn, 'JoinRoom', roomCode.toUpperCase(), result.token)
  }

  const handleStartGame = async () => {
    const conn = await connect()
    await invoke(conn, 'StartGame', store.roomCode)
  }

  const handleSubmitAnswer = async (answerIndex: number, responseTimeMs: number) => {
    const conn = await connect()
    await invoke(conn, 'SubmitAnswer', store.roomCode, answerIndex, responseTimeMs)
  }

  const handleLeaveGame = async () => {
    await disconnect()
    store.reset()
  }

  return {
    phase: store.phase,
    players: store.players,
    currentQuestion: store.currentQuestion,
    currentRound: store.currentRound,
    totalRounds: store.totalRounds,
    myScore: store.myScore,
    myStreak: store.myStreak,
    lastAnswerCorrect: store.lastAnswerCorrect,
    lastPoints: store.lastPoints,
    aiComment: store.aiComment,
    aiSummary: store.aiSummary,
    username: store.username,
    avatarColor: store.avatarColor,
    roomCode: store.roomCode,
    topic: store.topic,
    difficulty: store.difficulty,

    createRoom: handleCreateRoom,
    joinRoom: handleJoinRoom,
    startGame: handleStartGame,
    submitAnswer: handleSubmitAnswer,
    leaveGame: handleLeaveGame,
  }
}