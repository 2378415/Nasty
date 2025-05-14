import { HttpClient } from '@/@nasty/Axios';

export function getCurrentUserInfo(){
   return HttpClient.post("/Portal/User/GetCurrentUserInfo",{});
}