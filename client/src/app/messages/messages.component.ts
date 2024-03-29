import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { MessageService } from '../_services/message.service';
import { ConfirmService } from '../_services/confirm.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  pagination: Pagination;
  container: string = "Unread";
  pageNumber = 1;
  pageSize = 5;
  loadingFlag = false;

  constructor(private messageService: MessageService, private confirmService : ConfirmService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.loadingFlag = false;
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(response => {
      this.messages = response.result;
      //console.log(response.pagination.totalItems);
      this.pagination = response.pagination;
      this.loadingFlag = false;
    })
  }
  
  deleteMessage(id: number){
    this.confirmService.confirm('Confirm Delete Message', 'This can not be undone').subscribe(result => {
      if(result){
        this.messageService.deleteMessage(id).subscribe(() =>{
          this.messages.splice(this.messages.findIndex(m => m.id  === id), 1);
        })
      }
    });
  }


  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
