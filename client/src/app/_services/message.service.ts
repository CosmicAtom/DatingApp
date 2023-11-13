import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Message } from '../_models/message';
import { getPaginationHeader, getPagincationResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;

  constructor(private http : HttpClient) { }

  getMessages(pageNumber, pageSize, container){
    let params = getPaginationHeader(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPagincationResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(Username:string){
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + Username);
  }

  sendMessage(Username:string, content : string){
    return this.http.post<Message>(this.baseUrl + 'messages', { recipientUsername : Username, content});
  }

  deleteMessage(id : number){
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }

}
