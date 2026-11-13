import {
  AccountInfo,
  AuthenticationResult,
  BrowserCacheLocation,
  InteractionRequiredAuthError,
  PublicClientApplication
} from '@azure/msal-browser';

import { appUrlPath } from 'utils/url';
import { setAuthInfo } from 'data/auth/actions';
import { AuthInfo } from 'data/auth/models';

import { history } from './history';
import { store } from './store';

interface AuthSettings {
  mode: 'Directory' | 'EntraId';
  clientId?: string;
  authority?: string;
  scopes?: string[];
}

const apiUrl = appUrlPath + '/api/auth';
const returnPathKey = 'entraReturnPath';
const refreshIntervalMs = 4 * 60 * 1000;

let settings: AuthSettings = { mode: 'Directory' };
let msal: PublicClientApplication | undefined;

export const isEntraAuth = () => msal != null;

async function getCurrentUser(token: string): Promise<AuthInfo> {
  const response = await fetch(apiUrl + '/getCurrentUser', { headers: { Authorization: 'Bearer ' + token } });
  if (!response.ok) throw new Error('getCurrentUser failed: ' + response.status);
  const info = (await response.json()) as AuthInfo;
  return { ...info, isAuth: true, token };
}

async function applyResult(result: AuthenticationResult) {
  const info = await getCurrentUser(result.accessToken);
  store.dispatch(setAuthInfo({ ...info, expires: result.expiresOn ?? undefined }));
}

async function refreshToken() {
  const account: AccountInfo | null = msal?.getActiveAccount() ?? null;
  if (msal == null || account == null || !store.getState().auth.authInfo.isAuth) return;
  try {
    const result = await msal.acquireTokenSilent({ account, scopes: settings.scopes ?? [] });
    store.dispatch(
      setAuthInfo({
        ...store.getState().auth.authInfo,
        token: result.accessToken,
        expires: result.expiresOn ?? undefined
      })
    );
  } catch (e) {
    if (e instanceof InteractionRequiredAuthError) store.dispatch(setAuthInfo());
  }
}

/** Loads the server auth mode and, in Entra ID mode, completes any pending MSAL redirect sign-in. */
export async function initAuth() {
  try {
    const response = await fetch(apiUrl + '/getAuthSettings');
    if (response.ok) settings = await response.json();
  } catch {
    return;
  }
  if (settings.mode !== 'EntraId' || settings.clientId == null) return;

  msal = new PublicClientApplication({
    auth: {
      clientId: settings.clientId,
      authority: settings.authority,
      redirectUri: window.location.origin + appUrlPath + '/',
      navigateToLoginRequestUrl: false
    },
    cache: { cacheLocation: BrowserCacheLocation.SessionStorage }
  });
  await msal.initialize();

  const result = await msal.handleRedirectPromise();
  if (result != null) {
    msal.setActiveAccount(result.account);
    await applyResult(result);
    const returnPath = sessionStorage.getItem(returnPathKey) ?? '/';
    sessionStorage.removeItem(returnPathKey);
    history.replace(returnPath);
  }
  window.setInterval(refreshToken, refreshIntervalMs);
}

export function entraSignIn(returnPath: string) {
  if (msal == null) return;
  sessionStorage.setItem(returnPathKey, returnPath);
  msal.loginRedirect({ scopes: settings.scopes ?? [], prompt: 'select_account' });
}

/** Clears the cached Entra ID account locally (without a server round trip), so shared devices do not reuse it. */
export function entraClearAccount() {
  if (msal == null || msal.getAllAccounts().length === 0) return;
  msal.setActiveAccount(null);
  msal.logoutRedirect({ onRedirectNavigate: () => false });
}
