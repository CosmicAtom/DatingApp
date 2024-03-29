import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member : Member;

  constructor(private memberSerivce: MembersService, private toastrService : ToastrService, public presence : PresenceService){
  }

  ngOnInit(): void {
  }

  addLikes(member : Member){
    this.memberSerivce.addLikes(member.userName).subscribe(() => {
      this.toastrService.success('You have liked ' + member.knownAs);
    })
  }
}
