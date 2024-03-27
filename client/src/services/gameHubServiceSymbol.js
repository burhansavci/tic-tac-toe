import { HttpTransportType, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr'

export const gameHubServiceSymbol = Symbol('gameHubService')
export default class GameHubService {
  constructor() {
    this.connection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Trace)
      .withAutomaticReconnect()
      .withUrl('https://localhost:7031/hub/game', { transport: HttpTransportType.WebSockets, withCredentials: false })
      .build()
  }

  async startGameHub() {
    await this.connection.start()
  }

  async joinGame() {
    if (this.connection.state === HubConnectionState.Connected) {
      return await this.connection.invoke('JoinGame')
    } else {
      throw new Error('Connection not yet established')
    }
  }

}

export const ConnectionServices = {
  install: (app, options) => {

    app.provide(gameHubServiceSymbol, new GameHubService())
  }
}
