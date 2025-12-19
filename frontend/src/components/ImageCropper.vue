<template>
  <div class="relative">
    <div ref="cropperContainer" class="max-w-full relative inline-block">
      <img
        ref="imageElement"
        :src="imageUrl"
        alt="Image to crop"
        class="max-w-full h-auto block"
        @load="initializeCropper"
      />

      <!-- Crop rectangle overlay -->
      <div
        v-if="imageLoaded && cropRect"
        class="absolute inset-0 pointer-events-none overflow-hidden"
      >
        <!-- Crop rectangle -->
        <div
          ref="cropRectangle"
          class="absolute border-2 border-blue-500 bg-transparent cursor-move pointer-events-auto"
          :style="{
            left: cropRect.x + 'px',
            top: cropRect.y + 'px',
            width: cropRect.width + 'px',
            height: cropRect.height + 'px',
            boxShadow: '0 0 0 9999px rgba(0, 0, 0, 0.4)',
          }"
          @mousedown="startDrag"
        >
          <!-- Resize handles -->
          <div
            class="absolute -top-2 -left-2 w-4 h-4 bg-blue-500 border-2 border-white rounded-full cursor-nw-resize hover:bg-blue-600"
            @mousedown.stop="(event) => startResize(event, 'nw')"
          ></div>
          <div
            class="absolute -top-2 -right-2 w-4 h-4 bg-blue-500 border-2 border-white rounded-full cursor-ne-resize hover:bg-blue-600"
            @mousedown.stop="(event) => startResize(event, 'ne')"
          ></div>
          <div
            class="absolute -bottom-2 -left-2 w-4 h-4 bg-blue-500 border-2 border-white rounded-full cursor-sw-resize hover:bg-blue-600"
            @mousedown.stop="(event) => startResize(event, 'sw')"
          ></div>
          <div
            class="absolute -bottom-2 -right-2 w-4 h-4 bg-blue-500 border-2 border-white rounded-full cursor-se-resize hover:bg-blue-600"
            @mousedown.stop="(event) => startResize(event, 'se')"
          ></div>

          <!-- Dimension label -->
          <div
            class="absolute -top-8 left-0 bg-blue-500 text-white px-2 py-1 text-xs rounded whitespace-nowrap"
          >
            {{ Math.round(cropRect.width * imageScale) }} x
            {{ Math.round(cropRect.height * imageScale) }}
          </div>
        </div>
      </div>
    </div>

    <div class="mt-4">
      <div class="flex flex-wrap gap-2 items-center">
        <button
          @click="addCurrentCrop"
          :disabled="!imageLoaded"
          class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors flex items-center text-sm"
        >
          <Plus class="w-4 h-4 mr-1" />
          Add This Crop
        </button>

        <button
          @click="resetCropRect"
          :disabled="!imageLoaded"
          class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors flex items-center text-sm"
        >
          Reset Position
        </button>

        <button
          @click="clearAllAreas"
          :disabled="cropAreas.length === 0"
          class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors flex items-center text-sm"
        >
          <Trash2 class="w-4 h-4 mr-1" />
          Clear All
        </button>

        <div class="text-sm text-gray-600">
          Move and resize the rectangle, then click "Add This Crop"
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, computed } from 'vue'
import { Plus, Trash2 } from 'lucide-vue-next'
import type { CropArea } from '@/types'

interface Props {
  imageUrl: string
  cropAreas: CropArea[]
}

const props = defineProps<Props>()

const emit = defineEmits<{
  cropAreaAdded: [area: CropArea]
  cropAreaRemoved: [id: string]
  cropAreaUpdated: [id: string, area: Partial<CropArea>]
}>()

const imageElement = ref<HTMLImageElement>()
const cropperContainer = ref<HTMLDivElement>()
const cropRectangle = ref<HTMLDivElement>()
const imageLoaded = ref(false)
const isDragging = ref(false)
const isResizing = ref(false)
const resizeHandle = ref('')
const dragStart = ref({ x: 0, y: 0 })
const initialRect = ref({ x: 0, y: 0, width: 0, height: 0 })

// Default crop rectangle (display coordinates)
const cropRect = ref({ x: 50, y: 50, width: 200, height: 150 })

const imageScale = computed(() => {
  if (!imageElement.value || !imageLoaded.value) return 1
  return imageElement.value.naturalWidth / imageElement.value.getBoundingClientRect().width
})

const initializeCropper = () => {
  if (!imageElement.value) return

  imageLoaded.value = true
  resetCropRect()

  // Add global event listeners
  document.addEventListener('mousemove', handleGlobalMouseMove)
  document.addEventListener('mouseup', handleGlobalMouseUp)
}

const resetCropRect = () => {
  if (!imageElement.value) return

  const rect = imageElement.value.getBoundingClientRect()
  cropRect.value = {
    x: Math.max(0, (rect.width - 200) / 2),
    y: Math.max(0, (rect.height - 150) / 2),
    width: Math.min(200, rect.width - 20),
    height: Math.min(150, rect.height - 20),
  }
}

const startDrag = (event: MouseEvent) => {
  if (isResizing.value) return

  isDragging.value = true
  dragStart.value = { x: event.clientX, y: event.clientY }
  initialRect.value = { ...cropRect.value }
  event.preventDefault()
}

const startResize = (event: MouseEvent, handle: string) => {
  isResizing.value = true
  resizeHandle.value = handle
  isDragging.value = false
  dragStart.value = { x: event.clientX, y: event.clientY }
  initialRect.value = { ...cropRect.value }
  event.preventDefault()
}

const handleGlobalMouseMove = (event: MouseEvent) => {
  if (!isDragging.value && !isResizing.value) return
  if (!imageElement.value) return

  const deltaX = event.clientX - dragStart.value.x
  const deltaY = event.clientY - dragStart.value.y
  const imageRect = imageElement.value.getBoundingClientRect()

  if (isDragging.value) {
    // Move the rectangle
    const newX = Math.max(
      0,
      Math.min(initialRect.value.x + deltaX, imageRect.width - initialRect.value.width),
    )
    const newY = Math.max(
      0,
      Math.min(initialRect.value.y + deltaY, imageRect.height - initialRect.value.height),
    )

    cropRect.value.x = newX
    cropRect.value.y = newY
  } else if (isResizing.value) {
    // Resize the rectangle
    const newRect = { ...initialRect.value }

    switch (resizeHandle.value) {
      case 'nw':
        newRect.x = Math.max(0, initialRect.value.x + deltaX)
        newRect.y = Math.max(0, initialRect.value.y + deltaY)
        newRect.width = Math.max(20, initialRect.value.width - deltaX)
        newRect.height = Math.max(20, initialRect.value.height - deltaY)
        break
      case 'ne':
        newRect.y = Math.max(0, initialRect.value.y + deltaY)
        newRect.width = Math.max(20, initialRect.value.width + deltaX)
        newRect.height = Math.max(20, initialRect.value.height - deltaY)
        break
      case 'sw':
        newRect.x = Math.max(0, initialRect.value.x + deltaX)
        newRect.width = Math.max(20, initialRect.value.width - deltaX)
        newRect.height = Math.max(20, initialRect.value.height + deltaY)
        break
      case 'se':
        newRect.width = Math.max(20, initialRect.value.width + deltaX)
        newRect.height = Math.max(20, initialRect.value.height + deltaY)
        break
    }

    // Constrain to image bounds
    newRect.width = Math.min(newRect.width, imageRect.width - newRect.x)
    newRect.height = Math.min(newRect.height, imageRect.height - newRect.y)

    cropRect.value = newRect
  }
}

const handleGlobalMouseUp = () => {
  isDragging.value = false
  isResizing.value = false
  resizeHandle.value = ''
}

const addCurrentCrop = () => {
  if (!imageElement.value || !imageLoaded.value) return

  // Convert display coordinates to actual image coordinates
  const scale = imageScale.value
  const area: CropArea = {
    id: `crop-${Date.now()}`,
    x: Math.round(cropRect.value.x * scale),
    y: Math.round(cropRect.value.y * scale),
    width: Math.round(cropRect.value.width * scale),
    height: Math.round(cropRect.value.height * scale),
  }

  emit('cropAreaAdded', area)
}

const clearAllAreas = () => {
  props.cropAreas.forEach((area) => {
    emit('cropAreaRemoved', area.id)
  })
}

onMounted(() => {
  if (imageElement.value?.complete) {
    initializeCropper()
  }
})

onUnmounted(() => {
  document.removeEventListener('mousemove', handleGlobalMouseMove)
  document.removeEventListener('mouseup', handleGlobalMouseUp)
})

watch(
  () => props.imageUrl,
  () => {
    imageLoaded.value = false
    isDragging.value = false
    isResizing.value = false
  },
)
</script>
