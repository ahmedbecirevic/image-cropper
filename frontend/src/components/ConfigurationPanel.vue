<template>
  <div class="bg-white rounded-lg shadow-md p-6">
    <h2 class="text-xl font-semibold mb-4 flex items-center">
      <Settings class="mr-2" />
      Logo Overlay Configuration
    </h2>

    <div v-if="store.configurations.length > 0" class="mb-6">
      <label class="block text-sm font-medium text-gray-700 mb-2">
        Select Configuration (optional)
      </label>
      <select
        v-model="store.selectedConfig"
        class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
      >
        <option :value="null">No logo overlay</option>
        <option v-for="config in store.configurations" :key="config.id" :value="config.id">
          {{ config.logoPosition }} - Scale: {{ config.scaleDown }}
          {{ config.hasLogoImage ? '✓' : '✗' }}
        </option>
      </select>
    </div>

    <div class="border-t pt-6">
      <h3 class="text-lg font-medium mb-4">Create New Configuration</h3>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            Scale Down (0.01 - 0.25)
          </label>
          <input
            v-model.number="newConfig.scaleDown"
            type="number"
            min="0.01"
            max="0.25"
            step="0.01"
            required
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
          <p class="text-xs text-gray-500 mt-1">
            Controls the scaling of the final image (1% to 25%)
          </p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1"> Logo Position </label>
          <select
            v-model="newConfig.logoPosition"
            required
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="top-left">Top Left</option>
            <option value="top-right">Top Right</option>
            <option value="bottom-left">Bottom Left</option>
            <option value="bottom-right">Bottom Right</option>
            <option value="center">Center</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1"> Logo Image (PNG) </label>
          <div
            @click="triggerLogoFileInput"
            class="w-full px-3 py-2 border border-gray-300 rounded-md cursor-pointer hover:border-gray-400 focus-within:ring-2 focus-within:ring-blue-500 focus-within:border-blue-500 bg-gray-50 transition-colors"
          >
            <div class="flex items-center gap-2 text-gray-600">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
                ></path>
              </svg>
              <span class="text-sm">
                {{ newConfig.logoImage ? newConfig.logoImage.name : 'Choose logo file...' }}
              </span>
            </div>
          </div>
          <input
            ref="logoFileInput"
            type="file"
            accept="image/png"
            @change="handleLogoFileChange"
            class="hidden"
          />
          <p class="text-xs text-gray-500 mt-1">Optional PNG logo to overlay on cropped images</p>
        </div>

        <div class="flex gap-3">
          <button
            type="submit"
            :disabled="store.isLoading"
            class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors flex items-center text-sm"
          >
            <Plus class="w-4 h-4 mr-1" />
            <span v-if="store.isLoading">Creating...</span>
            <span v-else>Create Configuration</span>
          </button>

          <button
            type="button"
            @click="resetForm"
            class="px-4 py-2 bg-gray-100 text-gray-700 rounded hover:bg-gray-200 transition-colors text-sm"
          >
            Reset
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { Settings, Plus } from 'lucide-vue-next'
import { useImageCropperStore } from '@/stores/imageCropper'
import type { CreateConfigurationRequest } from '@/types'

const store = useImageCropperStore()

const logoFileInput = ref<HTMLInputElement>()

const newConfig = reactive<CreateConfigurationRequest>({
  scaleDown: 0.1,
  logoPosition: 'bottom-right',
  logoImage: undefined,
})

const triggerLogoFileInput = () => {
  logoFileInput.value?.click()
}

const handleLogoFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (file) {
    if (!file.type.startsWith('image/png')) {
      store.error = 'Please select a PNG image file for the logo'
      return
    }
    newConfig.logoImage = file
  }
}

const resetForm = () => {
  newConfig.scaleDown = 0.1
  newConfig.logoPosition = 'bottom-right'
  newConfig.logoImage = undefined

  if (logoFileInput.value) {
    logoFileInput.value.value = ''
  }
}

const handleSubmit = async () => {
  await store.createConfiguration({ ...newConfig })
  resetForm()
}
</script>
