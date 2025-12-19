import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import {
  PublicClientApplication,
  type AccountInfo,
  type SilentRequest,
  type PopupRequest,
  type EndSessionRequest,
} from '@azure/msal-browser'
import { msalConfig, loginRequest, apiRequest } from '@/config/authConfig'

export interface UserInfo {
  id: string
  displayName: string
  email: string
  givenName?: string
  surname?: string
}

export const useAuthStore = defineStore('auth', () => {
  const msalInstance = new PublicClientApplication(msalConfig)
  const account = ref<AccountInfo | null>(null)
  const userInfo = ref<UserInfo | null>(null)
  const isAuthenticated = ref(false)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const accessToken = ref<string | null>(null)

  const currentUser = computed(() => userInfo.value)
  const isSignedIn = computed(() => isAuthenticated.value && !!account.value)

  const initialize = async () => {
    try {
      await msalInstance.initialize()

      const response = await msalInstance.handleRedirectPromise()
      if (response) {
        setActiveAccount(response.account)
        await acquireTokenSilent()
      } else {
        // Check if there are cached accounts
        const accounts = msalInstance.getAllAccounts()
        if (accounts.length > 0) {
          setActiveAccount(accounts[0] ?? null)
          await acquireTokenSilent()
        }
      }
    } catch (err) {
      console.error('MSAL initialization error:', err)
      error.value = 'Failed to initialize authentication'
    }
  }

  const setActiveAccount = (selectedAccount: AccountInfo | null) => {
    account.value = selectedAccount
    isAuthenticated.value = !!selectedAccount

    if (selectedAccount) {
      userInfo.value = {
        id: selectedAccount.localAccountId,
        displayName: selectedAccount.name || '',
        email: selectedAccount.username || '',
        givenName: selectedAccount.idTokenClaims?.given_name as string,
        surname: selectedAccount.idTokenClaims?.family_name as string,
      }
    } else {
      userInfo.value = null
      accessToken.value = null
    }
  }

  const signIn = async () => {
    if (isLoading.value) return

    isLoading.value = true
    error.value = null

    try {
      const response = await msalInstance.loginPopup(loginRequest as PopupRequest)
      setActiveAccount(response.account)
      await acquireTokenSilent()
    } catch (err: unknown) {
      console.error('Sign in error:', err)
      const msalError = err as { errorCode?: string; errorMessage?: string }
      if (msalError.errorCode === 'popup_window_error' || msalError.errorCode === 'user_cancelled') {
        // Fallback to redirect
        await signInRedirect()
      } else {
        error.value = msalError.errorMessage || 'Sign in failed'
      }
    } finally {
      isLoading.value = false
    }
  }

  const signInRedirect = async () => {
    try {
      await msalInstance.loginRedirect(loginRequest)
    } catch (err: unknown) {
      console.error('Sign in redirect error:', err)
      error.value = (err as { errorMessage?: string })?.errorMessage || 'Sign in failed'
    }
  }

  const acquireTokenSilent = async (): Promise<string | null> => {
    if (!account.value) return null

    try {
      const request: SilentRequest = {
        ...apiRequest,
        account: account.value,
      }

      const response = await msalInstance.acquireTokenSilent(request)
      accessToken.value = response.accessToken
      return response.accessToken
    } catch (err) {
      console.warn('Silent token acquisition failed:', err)

      return await acquireTokenInteractive()
    }
  }

  const acquireTokenInteractive = async (): Promise<string | null> => {
    if (!account.value) return null

    try {
      const request: PopupRequest = {
        ...apiRequest,
        account: account.value,
      }

      const response = await msalInstance.acquireTokenPopup(request)
      accessToken.value = response.accessToken
      return response.accessToken
    } catch (err: unknown) {
      console.error('Interactive token acquisition failed:', err)
      const msalError = err as { errorMessage?: string }
      error.value = msalError.errorMessage || 'Token acquisition failed'
      return null
    }
  }

  // Get current access token
  const getAccessToken = async (): Promise<string | null> => {
    if (!account.value) return null

    if (accessToken.value) {
      return accessToken.value
    }

    return await acquireTokenSilent()
  }

  // Sign out
  const signOut = async () => {
    if (!account.value) return

    const request: EndSessionRequest = {
      account: account.value,
      postLogoutRedirectUri: msalConfig.auth.postLogoutRedirectUri,
    }

    try {
      await msalInstance.logoutPopup(request)
      setActiveAccount(null)
    } catch (err) {
      console.warn('Popup sign out failed, trying redirect:', err)
      await msalInstance.logoutRedirect(request)
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    account: computed(() => account.value),
    userInfo: computed(() => userInfo.value),
    isAuthenticated: computed(() => isAuthenticated.value),
    isLoading: computed(() => isLoading.value),
    error: computed(() => error.value),
    accessToken: computed(() => accessToken.value),
    currentUser,
    isSignedIn,
    initialize,
    signIn,
    signInRedirect,
    signOut,
    getAccessToken,
    clearError,
  }
})
