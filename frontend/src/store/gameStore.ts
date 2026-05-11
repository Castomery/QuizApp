import { create } from "zustand";
import * as signalR from '@microsoft/signalr'
import type {
  Player,
  Question,
  GamePhase,
  RoomStateData,
} from "../types/game.types";

interface GameState {
  // гравець
  username: string;
  avatarColor: string;
  token: string;
  playerId: string;

  // кімната
  roomCode: string;
  topic: string;
  difficulty: number;
  totalRounds: number;
  currentRound: number;

  // гра
  phase: GamePhase;
  players: Player[];
  currentQuestion: Question | null;
  lastAnswerCorrect: boolean | null;
  lastPoints: number;
  myScore: number;
  myStreak: number;
  connection: signalR.HubConnection | null

  // AI
  aiComment: string;
  aiSummary: string;

  // actions
  setPlayerInfo: (
    username: string,
    avatarColor: string,
    token: string,
    playerId: string,
  ) => void;
  setRoomCode: (code: string) => void;
  setPhase: (phase: GamePhase) => void;
  setRoomState: (data: RoomStateData) => void;
  addPlayer: (player: Player) => void;
  removePlayer: (username: string) => void;
  updateLeaderboard: (players: Player[]) => void;
  setQuestion: (question: Question, round: number) => void;
  setAnswerResult: (
    isCorrect: boolean,
    points: number,
    score: number,
    streak: number,
  ) => void;
  setRoundEnded: (comment: string) => void;
  setGameFinished: (players: Player[], summary: string) => void;
  setConnection: (connection: signalR.HubConnection | null) => void
  reset: () => void;
}

const initialState = {
  username: "",
  avatarColor: "",
  token: "",
  playerId: "",
  roomCode: "",
  topic: "",
  difficulty: 1,
  totalRounds: 5,
  currentRound: 1,
  phase: "lobby" as GamePhase,
  players: [],
  currentQuestion: null,
  lastAnswerCorrect: null,
  connection: null,
  lastPoints: 0,
  myScore: 0,
  myStreak: 0,
  aiComment: "",
  aiSummary: "",
};

export const useGameStore = create<GameState>((set) => ({
  ...initialState,

  setPlayerInfo: (username, avatarColor, token, playerId) =>
    set({ username, avatarColor, token, playerId }),

  setRoomCode: (roomCode) => set({ roomCode }),

  setPhase: (phase) => set({ phase }),

  setRoomState: (data: RoomStateData) =>
    set({
      topic: data.topic,
      difficulty: data.difficulty,
      players: data.players,
      roomCode: data.roomCode,
      phase: "waiting",
    }),

  addPlayer: (player) => set((s) => ({ players: [...s.players, player] })),

  removePlayer: (username) =>
    set((s) => ({ players: s.players.filter((p) => p.username !== username) })),

  updateLeaderboard: (players) => set({ players }),

  setQuestion: (question, round) =>
    set({
      currentQuestion: question,
      currentRound: round,
      lastAnswerCorrect: null,
      lastPoints: 0,
      phase: "playing",
    }),

  setAnswerResult: (isCorrect, points, score, streak) =>
    set({
      lastAnswerCorrect: isCorrect,
      lastPoints: points,
      myScore: score,
      myStreak: streak,
    }),

  setRoundEnded: (comment) => set({ aiComment: comment }),

  setConnection: (connection) => set({ connection }),

  setGameFinished: (players, summary) =>
    set({
      players,
      aiSummary: summary,
      phase: "results",
    }),

  reset: () => set(initialState),
}));
