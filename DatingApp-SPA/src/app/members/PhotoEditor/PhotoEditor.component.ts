import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/Photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/User.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './PhotoEditor.component.html',
  styleUrls: ['./PhotoEditor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver: false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(private authService: AuthService, private alertify: AlertifyService,
              private userService: UserService) { }

  ngOnInit() {
    this.initializerUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializerUploader() {
    this.uploader = new FileUploader({
      url : this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
        this.alertify.success('Photo(s) uploded successfully.');
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.SetMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMain = this.photos.filter(p => p.isMain === true)[0];
      this.currentMain.isMain = false;
      photo.isMain = true;
      this.authService.changeMemberPhoto(photo.url);
      this.authService.currenteUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currenteUser));
      /*this.getMemberPhotoChange.emit(photo.url); */
      this.alertify.success('Main photo seted');
    }, error => {
      this.alertify.error(error);
    });
}

deletePhoto(id: number) {
  this.alertify.confirm('Are you sure you want to delede this photo?', () => {
    this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
      this.alertify.success('Photo removed successfully');
    }, error => {
      this.alertify.error('Failed to delete the photo');
    });
  });
}

}
