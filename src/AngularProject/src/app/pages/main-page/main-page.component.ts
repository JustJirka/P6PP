import { Component, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common'
import { AppComponent } from '../../app.component';
import { NavigationComponent } from "../../components/navigation/navigation.component";
import { CalendarComponent } from "../../components/calendar/calendar.component";
import { FooterComponent } from '../../components/footer/footer.component';
import { CourseComponent } from '../../components/course/course.component';

import { Course } from '../../services/interfaces/course';

import { CourseService } from '../../services/course.service';


@Component({
  selector: 'app-main-page',
  imports: [FooterComponent,NavigationComponent, CommonModule, CalendarComponent, CourseComponent],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss'
})
export class MainPageComponent {
  @ViewChild('bgVideo') bgVideoRef!: ElementRef<HTMLVideoElement>;
  isMuted = true;
  isHidden = false;
  currentVideoTime = 0;


  constructor(
    private courseService: CourseService,
  ){

  }
  
  //courses: Course[] = [];

  // TESTING DATA !!!COMMENT 31:3!!!
  courses: Course[] = [
    {
      id: 1,
      trainerId: 1,
      start: new Date(),
      end: new Date(),
      price: 100,
      serviceName: "Swimming",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Swimming Pool 1",
      isCancelled: false
    },
    {
      id: 2,
      trainerId: 1,
      start: new Date(),
      end: new Date(),
      price: 200,
      serviceName: "Yoga",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Room 1",
      isCancelled: false
    },
    {
      id: 3,
      trainerId: 1,
      start: new Date(),
      end: new Date(),
      price: 250,
      serviceName: "Boxing",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Room 3",
      isCancelled: false
    },
    {
      id: 4,
      trainerId: 2,
      start: new Date(),
      end: new Date(),
      price: 50,
      serviceName: "Group training with Ronnie Coleman",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Room 6",
      isCancelled: false
    },
    {
      id: 5,
      trainerId: 2,
      start: new Date(),
      end: new Date(),
      price: 150,
      serviceName: "Powerlifting",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Room 2",
      isCancelled: false
    },
  ];

  ngOnInit(){
    this.courseService.getAllCourses().subscribe(courcesResponse => {
      this.courses = courcesResponse.data;
      console.log("course arr: ", this.courses);
    });
    
    //this.courseService.getAllCources().then((coursesList: Course[] = this.courses) => this.courses = coursesList)
  }

  scrollDown() {
    const cont = document.querySelector('.scroll-container');
    if(cont){
      (cont as HTMLElement).scrollBy({
        top: window.innerHeight,
        behavior: 'smooth'
      })
    }
    window.scrollTo({ top: window.innerHeight, behavior: 'smooth' });
  }
  ngAfterViewInit() {
    const video = this.bgVideoRef?.nativeElement;
    if (video) {
      video.muted = true;
      video.autoplay = true;
      video.play().catch(error => {
        console.error('Autoplay failed:', error);
      });
    }
  }

  toggleMute() {
    const video = this.bgVideoRef.nativeElement;
    this.isMuted = !this.isMuted;
    video.muted = this.isMuted;
  }

  toggleVideo() {
    const video = this.bgVideoRef?.nativeElement;
  
    if (video) {
      if (!this.isHidden) {
       
        this.currentVideoTime = video.currentTime; // сохранить позицию
        video.pause();
      }
    }
  
    this.isHidden = !this.isHidden;
  
    setTimeout(() => {
      const newVideo = this.bgVideoRef?.nativeElement;
      if (newVideo) {
        if (!this.isHidden) {
          newVideo.currentTime = this.currentVideoTime; // восстановить позицию
          newVideo.muted = this.isMuted;
          newVideo.autoplay = true;
          newVideo.play().catch(error => {
            console.error('Autoplay after show failed:', error);
          });
        } else {
          newVideo.pause();
        }
      }
    }, 0);
  }
  
  
  onVideoEnded() {
    this.isHidden = true;
  }
}
