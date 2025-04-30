import { Routes } from '@angular/router';
import { AppComponent } from './app.component';

import { NavigationComponent } from './components/navigation/navigation.component';
import { LoginPage } from './pages/login/login.page';
import { SignupPage } from './pages/signup/signup.page';
import { ProfilePage } from './pages/profile/profile.page';
import { MainPageComponent } from './pages/main-page/main-page.component'
import { PaymentComponent } from './pages/payment/payment.component';
import { ForgetPassComponent } from './pages/forget-pass/forget-pass.component';

import { AuthGuard } from './services/auth.guard';
import { ResetPassComponent } from './pages/reset-pass/reset-pass.component';
import { CoursePageComponent } from './pages/course-page/course-page.component';
import { VerificationComponent } from './pages/verification/verification.component';
import { AnalyticsComponent } from './pages/analytics/analytics.component';


const routeConfig: Routes = [
    {
        path: '',
        component: MainPageComponent,
        // loadChildren: () => import('./navigation/navigation.component').then(m => m.NavigationComponent),
        title: 'Main page'
    },
    {
        path: 'login',
        component: LoginPage,
        //loadChildren: () => import('./pages/login/login.page').then(m => m.LoginPage),
        title: 'Log in'
    },
    {
        path: 'signup',
        component: SignupPage,
        //loadChildren: () => import('./pages/signup/signup.page').then(m => m.SignupPage),
        title: 'Sign up'
    },
    {
        path: 'profile',
        component: ProfilePage,
        //loadChildren: () => import('./pages/signup/signup.page').then(m => m.SignupPage),
        title: 'Profile',
        canActivate: [AuthGuard] 
    },
    {
        path: 'payment',
        component: PaymentComponent,
        title: 'Get Robux',
        canActivate: [AuthGuard] 
    },
    {
        // to write a email for reset link
        path: 'password_forget',
        component: ForgetPassComponent,
        title: 'Forgot a password?'
    },
    {
        // reset link
        path: 'password_reset',
        component: ResetPassComponent,
        title: 'Reset a password'
    },
    {
        path: 'email_verification',
        component: VerificationComponent,
        title: 'Verify your account'
    },
    {
        path: 'course/:id',
        component: CoursePageComponent,
        title: 'Cource'
    },
    {
        path: 'analytics',
        component: AnalyticsComponent,
        title: 'Analytics'
    }
];

export default routeConfig;
