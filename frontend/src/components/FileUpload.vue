<template>
  <div class="relative">
    <div
      @dragover.prevent
      @dragenter.prevent
      @drop.prevent="handleDrop"
      @click="triggerFileInput"
      class="border-2 border-dashed border-gray-300 rounded-lg p-8 text-center hover:border-gray-400 transition-colors cursor-pointer"
      :class="{
        'border-blue-400 bg-blue-50': isDragging,
        'border-gray-300': !isDragging,
      }"
    >
      <Upload class="mx-auto h-12 w-12 text-gray-400 mb-4" />
      <div class="text-lg font-medium text-gray-900 mb-2">Drop your PNG image here</div>
      <div class="text-sm text-gray-600 mb-4">or click to browse files</div>
      <div class="text-xs text-gray-500">Supports PNG format only</div>
    </div>

    <input
      ref="fileInput"
      type="file"
      accept="image/png"
      @change="handleFileInput"
      class="hidden"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Upload } from 'lucide-vue-next'

const emit = defineEmits<{
  fileSelected: [file: File]
  error: [message: string]
}>()

const fileInput = ref<HTMLInputElement>()
const isDragging = ref(false)

const triggerFileInput = () => {
  fileInput.value?.click()
}

const validateFile = (file: File | undefined): file is File => {
  if (!file) {
    emit('error', 'No file selected')
    return false
  }

  if (!file.type.startsWith('image/png')) {
    emit('error', 'Please select a PNG image file')
    return false
  }

  if (file.size > 50 * 1024 * 1024) {
    emit('error', 'File size must be less than 50MB')
    return false
  }

  return true
}

const handleFileInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (validateFile(file)) {
    emit('fileSelected', file)
  }
}

const handleDrop = (event: DragEvent) => {
  isDragging.value = false

  const files = event.dataTransfer?.files
  if (files && files.length > 0) {
    const file = files[0]
    if (validateFile(file)) {
      emit('fileSelected', file)
    }
  }
}
</script>
