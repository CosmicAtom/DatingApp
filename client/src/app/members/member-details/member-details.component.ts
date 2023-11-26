import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})

export class MemberDetailsComponent implements OnInit,OnDestroy {
  @ViewChild('memberTabs', {static : true}) memberTabs : TabsetComponent;
  member : Member;
  galleryOptions : NgxGalleryOptions[];
  galleryImages : NgxGalleryImage[];
  activeTab : TabDirective;
  messages : Message[] = [];
  user : User;

  constructor( private route : ActivatedRoute, private messageService : MessageService,
     public presence : PresenceService, private accountService : AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
     }
  

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

    this.galleryOptions = [
      {
        width: '600px',
        height: '400px',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide
      }];
    
      this.galleryImages = this.getImages();
  }

  getImages() : NgxGalleryImage[] {
    const imagesUrl = [];
    for(const photo of this.member.photos){
      imagesUrl.push({
        small : photo?.url,
        medium : photo?.url,
        big : photo?.url,
      })
    }

    return imagesUrl;
  }

  laodMessages(){
    this.messageService.getMessageThread(this.member.userName).subscribe(messages => {
      this.messages = messages;
    })
  }

  selectTab(tabsId : number){
    this.memberTabs.tabs[tabsId].active = true;
  }

  onTabActivated(data : TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === 'Message' && this.messages.length === 0){
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
