import { apiClient } from './apiClient'
import type { CreateRoomResponse, JoinRoomResponse } from '../types/game.types'

export const createRoom = async (topic: string, difficulty: number): Promise<CreateRoomResponse> => {
  const { data } = await apiClient.post('/rooms', { topic, difficulty })
  return data
}

export const joinRoom = async (roomCode: string, playerName: string): Promise<JoinRoomResponse> => {
  const { data } = await apiClient.post(`/rooms/${roomCode}/join`, { playerName })
  return data
}