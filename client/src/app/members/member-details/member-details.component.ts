import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})

export class MemberDetailsComponent implements OnInit {
  member : Member;
  galleryOptions : NgxGalleryOptions[];
  galleryImages : NgxGalleryImage[];

  constructor(private memberService : MembersService, private route : ActivatedRoute) {}

  ngOnInit(): void {
    this.loadMembers();
    this.galleryOptions = [
      {
        width: '600px',
        height: '400px',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide
      }];
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

  loadMembers(){
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(member => {
      this.member = member;
      this.galleryImages = this.getImages();
    })
  }
}
