<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import SignIn from '@/components/SignIn.vue'
import UserHeader from '@/components/UserHeader.vue'
import ImageCropperApp from '@/components/ImageCropperApp.vue'

const authStore = useAuthStore()

const showApp = computed(() => authStore.isSignedIn && !authStore.isLoading)
const showSignIn = computed(() => !authStore.isSignedIn && !authStore.isLoading)
const showLoading = computed(() => authStore.isLoading)

onMounted(async () => {
  await authStore.initialize()
})
</script>

<template>
  <div id="app" class="min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="showLoading" class="min-h-screen flex items-center justify-center">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
        <p class="mt-4 text-gray-600">Loading...</p>
      </div>
    </div>

    <!-- Sign In Screen -->
    <SignIn v-else-if="showSignIn" />

    <!-- Main Application -->
    <div v-else-if="showApp">
      <UserHeader />
      <div class="container mx-auto px-4 py-8">
        <ImageCropperApp />
      </div>
    </div>
  </div>
</template>
