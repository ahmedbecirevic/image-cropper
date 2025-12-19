<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-bold text-gray-900">Sign in to your account</h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Use your Microsoft account to access the Image Cropper application
        </p>
      </div>

      <div class="mt-8 space-y-6">
        <!-- Error Message -->
        <div v-if="authStore.error" class="bg-red-50 border border-red-200 rounded-md p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <AlertCircle class="h-5 w-5 text-red-400" />
            </div>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-red-800">Authentication Error</h3>
              <div class="mt-2 text-sm text-red-700">
                {{ authStore.error }}
              </div>
              <div class="mt-3">
                <button
                  @click="authStore.clearError"
                  class="text-sm font-medium text-red-800 hover:text-red-600 underline"
                >
                  Dismiss
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Sign In Button -->
        <div>
          <button
            @click="handleSignIn"
            :disabled="authStore.isLoading"
            class="group relative w-full flex justify-center py-3 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors"
          >
            <span class="absolute left-0 inset-y-0 flex items-center pl-3">
              <div
                v-if="authStore.isLoading"
                class="animate-spin rounded-full h-5 w-5 border-b-2 border-white"
              ></div>
              <LogIn v-else class="h-5 w-5" />
            </span>
            {{ authStore.isLoading ? 'Signing in...' : 'Sign in with Microsoft' }}
          </button>
        </div>

        <!-- Alternative Sign In Method -->
        <div class="text-center">
          <button
            @click="authStore.signInRedirect"
            :disabled="authStore.isLoading"
            class="text-sm text-blue-600 hover:text-blue-500 disabled:text-gray-400 disabled:cursor-not-allowed"
          >
            Having trouble? Try redirect sign-in
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { LogIn, AlertCircle } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const handleSignIn = async () => {
  try {
    await authStore.signIn()
  } catch (error) {
    console.error('Sign in failed:', error)
  }
}
</script>
