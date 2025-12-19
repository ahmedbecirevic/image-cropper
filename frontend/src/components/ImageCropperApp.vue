<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">Image Cropper</h1>
        <p class="text-gray-600">
          Upload a PNG image and select crop areas to generate previews and final images
        </p>
      </div>

      <div v-if="store.error" class="mb-6 bg-red-50 border border-red-200 rounded-md p-4">
        <div class="flex">
          <AlertCircle class="h-5 w-5 text-red-400 mr-3" />
          <div class="text-sm text-red-700">{{ store.error }}</div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow-md p-6 mb-8">
        <h2 class="text-xl font-semibold mb-4 flex items-center">
          <Upload class="mr-2" />
          Upload Image
        </h2>

        <div class="flex flex-col sm:flex-row gap-4 items-start">
          <div class="flex-1">
            <FileUpload @file-selected="handleFileUpload" />
          </div>

          <div v-if="store.hasImage" class="flex gap-2">
            <button
              @click="store.clearAll"
              class="px-4 py-2 text-sm bg-gray-100 text-gray-700 rounded hover:bg-gray-200 transition-colors flex items-center"
            >
              <X class="w-4 h-4 mr-1" />
              Clear All
            </button>
          </div>
        </div>

        <div v-if="store.hasImage" class="mt-4 p-4 bg-gray-50 rounded-md">
          <p class="text-sm text-gray-600">
            <strong>File:</strong> {{ store.uploadedImage?.name }} ({{
              formatFileSize(store.uploadedImage?.size || 0)
            }})
          </p>
        </div>
      </div>

      <div v-if="store.hasImage" class="bg-white rounded-lg shadow-md p-6 mb-8">
        <h2 class="text-xl font-semibold mb-4 flex items-center">
          <Crop class="mr-2" />
          Select Crop Areas
        </h2>

        <ImageCropper
          :image-url="store.imageUrl"
          :crop-areas="store.cropAreas"
          @crop-area-added="store.addCropArea"
          @crop-area-removed="store.removeCropArea"
          @crop-area-updated="store.updateCropArea"
        />

        <div class="mt-4 flex flex-wrap gap-2">
          <div
            v-for="area in store.cropAreas"
            :key="area.id"
            class="px-3 py-1 rounded-full flex items-center text-gray-600"
          >
            Crop {{ `${area.width}x${area.height}` }}
            <button
              @click="store.removeCropArea(area.id)"
              class="text-blue-200 hover:text-white transition-colors"
            >
              <X />
            </button>
          </div>
        </div>

        <div class="mt-4 text-sm text-gray-600">
          <p><strong>Selected areas:</strong> {{ store.cropAreas.length }}</p>
          <p v-if="store.cropAreas.length < 3" class="text-amber-600">
            ⚠️ At least 3 crop areas required for preview
          </p>
        </div>
      </div>
      <ConfigurationPanel v-if="store.hasImage" class="mb-8" />
      <div v-if="store.hasImage" class="bg-white rounded-lg shadow-md p-6 mb-8">
        <h2 class="text-xl font-semibold mb-4 flex items-center">
          <ImageIcon class="mr-2" />
          Actions
        </h2>

        <div class="flex flex-wrap gap-4">
          <button
            @click="store.generatePreviews"
            :disabled="!store.canPreview || store.isLoading"
            class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors flex items-center"
          >
            <Eye class="w-5 h-5 mr-2" />
            <span v-if="store.isLoading">Generating...</span>
            <span v-else>Generate Preview</span>
          </button>

          <button
            @click="store.generateImages"
            :disabled="!store.canGenerate || store.isLoading"
            class="px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors flex items-center"
          >
            <Download class="w-5 h-5 mr-2" />
            <span v-if="store.isLoading">Generating...</span>
            <span v-else>Generate Final Images</span>
          </button>
        </div>
      </div>
      <PreviewResults
        v-if="store.previews.length > 0"
        :previews="store.previews"
        title="Preview Results (5% Scale)"
        class="mb-8"
      />
      <PreviewResults
        v-if="store.generatedImages.length > 0"
        :previews="store.generatedImages"
        title="Generated Images (Full Quality)"
        :show-download="true"
        @download="(imageData, index) => store.downloadImage(imageData, index)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { Upload, X, Crop, Eye, Download, AlertCircle, Image as ImageIcon } from 'lucide-vue-next'
import { useImageCropperStore } from '@/stores/imageCropper'
import FileUpload from '@/components/FileUpload.vue'
import ImageCropper from '@/components/ImageCropper.vue'
import ConfigurationPanel from '@/components/ConfigurationPanel.vue'
import PreviewResults from '@/components/PreviewResults.vue'

const store = useImageCropperStore()

onMounted(() => {
  store.loadConfigurations()
})

const handleFileUpload = (file: File) => {
  store.setImage(file)
}

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}
</script>
