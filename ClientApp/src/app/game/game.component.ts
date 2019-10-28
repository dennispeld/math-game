import {Component, OnInit} from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { Observable, of, timer } from 'rxjs';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  public hubConnection: HubConnection;

  // player
  public username: string;
  public playerID: string;
  public score: number;
  public playersOnline: number;

  // question
  public question: string;
  public answer: boolean;
  public roundEnded: boolean;

  // answer
  public answeredCorrect: boolean;
  public answeredResult: string;
  public answerColor: string;

  public joined: boolean;
  public notifications: string[] = [];

  ngOnInit() {
    document.getElementById('not-joined').style.display = 'block';
    document.getElementById('joined').style.display = 'none';
    document.getElementById('readyToAnswer').style.display = 'none';
    document.getElementById('rejected').style.display = 'none';

    this.score = 0;

    const builder = new HubConnectionBuilder();
    // as per setup in the startup.cs
    this.hubConnection = builder.withUrl('/game').build();


    // LISTENERS OF THE SERVER EVENTS

    // get information of a player, who has just joined the game
    this.hubConnection.on('acceptEntry', (username, playerID) => {
      this.username = username;
      this.playerID = playerID;
      this.joined = true;

      document.getElementById('readyToAnswer').style.display = 'block';
      document.getElementById('not-joined').style.display = 'none';
      document.getElementById('joined').style.display = 'block';
      document.getElementById('rejected').style.display = 'none';
    });

    // reject an entry for the player due to limit of number of connections
    this.hubConnection.on('rejectEntry', () => {
      document.getElementById('rejected').style.display = 'block';
      this.joined = false;
    });

    // get a current question or generate a new one
    this.hubConnection.on('getQuestion', (question, answer) => {
      this.roundEnded = false;
      this.question = question;
      this.answer = answer;
    });

    // update the score of a player according to their answer
    this.hubConnection.on('updateScore', (score) => {
      this.score = score;
    });

    // start a countdown
    this.hubConnection.on('startCountDown', () => {
      this.roundEnded = true;

      const source = timer(5000, 1000);
      source.subscribe(t => {
        // when a tick reaches 0, get a new question
        if (t === 0) {
          this.getQuestion();
          document.getElementById('readyToAnswer').style.display = 'block';
          document.getElementById('answerButtons').style.display = 'inline';
          document.getElementById('result').style.display = 'none';
        }

      });
    });

    // update notifications
    this.hubConnection.on('updateNotifications', (notification) => {
      if (this.joined) {
        this.notifications.reverse(); // reverse the notification to add the new one at the end
        this.notifications.push(notification);
        this.notifications.reverse(); // reverse it back to see the newes notification on the top

        if (this.notifications.length > 5) {
          this.notifications.length = 5; // show only latest 5 notifications
        }
      }
    });

    // show the number of players online
    this.hubConnection.on('showPlayersOnline', (playersOnline) => {
      this.playersOnline = playersOnline;
    });

    // starting the connection and get the current question rightaway
    this.hubConnection.start().then(() => this.getQuestion());

    document.getElementById('result').style.display = 'none';
  }

  // METHODS TO SEND INFORMATION FROM CLIENT TO SERVER

  // a new player joins the game
  join() {
    if (this.username !== '' && this.username.length !== 0 && this.username.trim() !== '') {
      this.hubConnection.invoke('JoinGame', this.username);
    }
  }

  // ask server to get the current question
  getQuestion() {
    this.hubConnection.invoke('GetQuestion');
  }

  // send the player's answer to the server
  setResult(iscorrect: boolean) {
    this.hubConnection.invoke('SendAnswer', this.question, this.answeredCorrect, this.score, this.username);

    document.getElementById('answerButtons').style.display = 'none';
    document.getElementById('result').style.display = 'block';

    this.answeredResult = iscorrect ? 'Correct' : 'Wrong';
    this.answerColor = iscorrect ? 'success' : 'danger';
  }

  // HELPER METHODS

  // a plyers answers YES
  answerYes() {
    this.answeredCorrect = this.answer === true;
    this.setResult(this.answeredCorrect);
  }

  // a player answers NO
  answerNo() {
    this.answeredCorrect = this.answer !== true;
    this.setResult(this.answeredCorrect);
  }
}
