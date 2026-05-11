import * as signalR from "@microsoft/signalr";
import { useGameStore } from "../store/gameStore";
import { config } from "../config/env";
import type { Player, Question, RoomStateData } from "../types/game.types";

export const useSignalR = () => {
  const store = useGameStore();
  const connection = store.connection;

  const connect = async (): Promise<signalR.HubConnection> => {
    if (connection) {
      if (connection.state === signalR.HubConnectionState.Connected){
          return connection;
      }
    }

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(config.hubUrl)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    // ── події від сервера ──────────────────────────────────────

    newConnection.on("RoomState", (data: RoomStateData) => {
      store.setRoomState(data);
    });

    newConnection.on(
      "PlayerJoined",
      (data: { username: string; avatarColor: string }) => {
        store.addPlayer({
          username: data.username,
          avatarColor: data.avatarColor,
          score: 0,
        });
      },
    );

    newConnection.on("PlayerLeft", (data: { username: string }) => {
      store.removePlayer(data.username);
    });

    newConnection.on("GeneratingQuestions", () => {
      store.setPhase("generating");
    });

    newConnection.on("GameStarted", () => {
      store.setPhase("playing");
    });

    newConnection.on(
      "QuestionStarted",
      (data: {
        text: string;
        options: string[];
        timeoutSec: number;
        round: number;
      }) => {
        const question: Question = {
          text: data.text,
          options: data.options,
          timeoutSec: data.timeoutSec,
        };
        store.setQuestion(question, data.round);
      },
    );

    newConnection.on("AnswerResult", (data) => {
      store.setAnswerResult(
        data.isCorrect,
        data.pointsEarned,
        data.newScore,
        data.streak,
      );
    });

    newConnection.on("LeaderboardUpdated", (players: Player[]) => {
      store.updateLeaderboard(players);
    });

    newConnection.on("RoundEnded", (data) => {
      store.setRoundEnded(data.aiComment);
      store.setPhase("roundEnd");
    });

    newConnection.on("GameFinished", (data) => {
      store.setGameFinished(data.leaderboard, data.aiSummary);
    });

    await newConnection.start();
    store.setConnection(newConnection);

    return newConnection;

  };

  const disconnect = async () => {
    await connection?.stop();
    store.setConnection(null);
  };

  const invoke = async (
    conn: signalR.HubConnection,
    method: string,
    ...args: unknown[]
  ) => {
    if (conn.state !== signalR.HubConnectionState.Connected) {
      throw new Error(`SignalR: стан ${conn.state} при виклику ${method}`);
    }
    await conn.invoke(method, ...args);
  };

  return { connect, disconnect, invoke };
};

