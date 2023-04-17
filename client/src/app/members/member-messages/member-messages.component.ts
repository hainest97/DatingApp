import { ChangeDetectionStrategy, Component, Input, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent{
  @ViewChild('messageForm') messageForm?: NgForm
  @Input() username?: string;
  messageContent = '';

  constructor(public messageService: MessageService) {}

  sendMessage(){
    if(!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm?.reset();
    })
  }

  
}
