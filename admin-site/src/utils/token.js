import { getCookie, setCookie, removeCookie } from './cookie';

export const tokenHelper = {
  getAccess:  () => getCookie("accessToken"),
  getRefresh: () => getCookie("refreshToken"),
 
  save: (tokenModel) => {
    setCookie("accessToken",  tokenModel.accessToken,  tokenModel.accessTokenExpireIn);
    setCookie("refreshToken", tokenModel.refreshToken, tokenModel.refreshTokenExpireIn);
  },
 
  clear: () => {
    removeCookie("accessToken");
    removeCookie("refreshToken");
  },
};