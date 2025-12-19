<template>
  <div class="bg-white rounded-lg shadow-md p-6">
    <h2 class="text-xl font-semibold mb-4 flex items-center">
      <ImageIcon class="mr-2" />
      {{ title }}
    </h2>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <div
        v-for="(result, index) in previews"
        :key="index"
        class="border border-gray-200 rounded-lg overflow-hidden"
      >
        <div v-if="result.error" class="p-4 bg-red-50 text-red-700 text-sm">
          <AlertCircle class="w-4 h-4 inline mr-2" />
          {{ result.error }}
        </div>
        <div v-else>
          <div class="aspect-square bg-gray-100 flex items-center justify-center overflow-hidden">
            <img
              :src="`data:${result.contentType};base64,${result.imageData}`"
              :alt="`Crop ${index + 1}`"
              class="max-w-full max-h-full object-contain"
            />
          </div>

          <div class="p-4 space-y-2">
            <div class="text-xs text-gray-600">
              <div>
                <strong>Position:</strong> ({{ result.coordinates.x }}, {{ result.coordinates.y }})
              </div>
              <div>
                <strong>Size:</strong> {{ result.coordinates.width }} ×
                {{ result.coordinates.height }}
              </div>
              <div v-if="result.hasLogoOverlay" class="text-green-600">
                <strong>✓ Logo overlay applied</strong>
              </div>
            </div>

            <button
              v-if="showDownload"
              @click="$emit('download', result.imageData, index)"
              class="w-full px-3 py-2 bg-blue-600 text-white rounded text-sm hover:bg-blue-700 transition-colors flex items-center justify-center"
            >
              <Download class="w-4 h-4 mr-1" />
              Download
            </button>
          </div>
        </div>
      </div>
    </div>
    <div class="mt-6 p-4 bg-gray-50 rounded-lg">
      <div class="text-sm text-gray-600">
        <strong>Results:</strong>
        {{ successfulResults }} successful, {{ errorResults }} errors out of
        {{ previews.length }} total
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Image as ImageIcon, Download, AlertCircle } from 'lucide-vue-next'
import type { ImageCropResponse } from '@/types'

interface Props {
  previews: ImageCropResponse[]
  title: string
  showDownload?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showDownload: false,
})

defineEmits<{
  download: [imageData: string, index: number]
}>()

const successfulResults = computed(() => props.previews.filter((result) => !result.error).length)

const errorResults = computed(() => props.previews.filter((result) => !!result.error).length)
</script>
