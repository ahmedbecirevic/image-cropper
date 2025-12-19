<template>
  <div class="bg-white border-b border-gray-200 px-4 py-3">
    <div class="flex items-center justify-between">
      <!-- User Info -->
      <div class="flex items-center space-x-3">
        <div class="flex items-center space-x-2">
          <div class="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
            <span class="text-white text-sm font-medium">
              {{ initials }}
            </span>
          </div>
          <div>
            <p class="text-sm font-medium text-gray-900">
              {{ authStore.userInfo?.displayName }}
            </p>
            <p class="text-xs text-gray-500">
              {{ authStore.userInfo?.email }}
            </p>
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex items-center space-x-2">
        <button
          @click="authStore.signOut"
          :disabled="authStore.isLoading"
          class="inline-flex items-center px-3 py-1.5 border border-transparent text-xs font-medium rounded text-white bg-red-600 hover:bg-red-700 disabled:bg-gray-300 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-red-500 transition-colors"
        >
          <LogOut class="w-3 h-3 mr-1" />
          Sign Out
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { LogOut } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const initials = computed(() => {
  const user = authStore.userInfo
  if (!user?.displayName) return 'U'

  const names = user.displayName.split(' ')
  if (names.length >= 2 && names[0] && names[1]) {
    return `${names[0][0]}${names[1][0]}`.toUpperCase()
  }
  return user.displayName[0]?.toUpperCase()
})
</script>
