import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type {
  CropArea,
  ImageCropResponse,
  Configuration,
  CreateConfigurationRequest,
} from '@/types'
import { ApiService } from '@/services/api'
import { getErrorMessage } from '@/utils/error'

export const useImageCropperStore = defineStore('imageCropper', () => {
  const uploadedImage = ref<File | null>(null)
  const imageUrl = ref<string>('')
  const cropAreas = ref<CropArea[]>([])
  const previews = ref<ImageCropResponse[]>([])
  const generatedImages = ref<ImageCropResponse[]>([])
  const configurations = ref<Configuration[]>([])
  const selectedConfig = ref<number | null>(null)
  const isLoading = ref(false)
  const error = ref<string>('')

  const hasImage = computed(() => uploadedImage.value !== null)
  const hasCropAreas = computed(() => cropAreas.value.length > 0)
  const canPreview = computed(() => hasImage.value && cropAreas.value.length >= 3)
  const canGenerate = computed(() => hasImage.value && hasCropAreas.value)

  const setImage = (file: File) => {
    uploadedImage.value = file
    imageUrl.value = URL.createObjectURL(file)
    // Clear previous data when new image is set
    clearCropData()
  }

  const addCropArea = (area: CropArea) => {
    cropAreas.value.push(area)
  }

  const removeCropArea = (id: string) => {
    cropAreas.value = cropAreas.value.filter((area) => area.id !== id)
  }

  const updateCropArea = (id: string, updatedArea: Partial<CropArea>) => {
    const index = cropAreas.value.findIndex((area) => area.id === id)
    if (index !== -1) {
      cropAreas.value[index] = { ...cropAreas.value[index], ...updatedArea } as CropArea
    }
  }

  const clearCropData = () => {
    cropAreas.value = []
    previews.value = []
    generatedImages.value = []
    error.value = ''
  }

  const clearAll = () => {
    uploadedImage.value = null
    imageUrl.value = ''
    clearCropData()
  }

  const generatePreviews = async () => {
    if (!uploadedImage.value || cropAreas.value.length < 3) {
      error.value = 'Please upload an image and select at least 3 crop areas'
      return
    }

    try {
      isLoading.value = true
      error.value = ''

      const request = {
        image: uploadedImage.value,
        cropCoordinates: cropAreas.value.map((area) => ({
          x: area.x,
          y: area.y,
          width: area.width,
          height: area.height,
        })),
      }

      previews.value = await ApiService.previewImage(request)
    } catch (err: unknown) {
      error.value = getErrorMessage(err, 'Failed to generate previews')
      console.error('Preview error:', err)
    } finally {
      isLoading.value = false
    }
  }

  const generateImages = async () => {
    if (!uploadedImage.value || cropAreas.value.length === 0) {
      error.value = 'Please upload an image and select crop areas'
      return
    }

    try {
      isLoading.value = true
      error.value = ''

      const request = {
        image: uploadedImage.value,
        cropCoordinates: cropAreas.value.map((area) => ({
          x: area.x,
          y: area.y,
          width: area.width,
          height: area.height,
        })),
      }

      generatedImages.value = await ApiService.generateImage(
        request,
        selectedConfig.value || undefined,
      )
    } catch (err: unknown) {
      error.value = getErrorMessage(err, 'Failed to generate images')
      console.error('Generate error:', err)
    } finally {
      isLoading.value = false
    }
  }

  const downloadImage = (imageData: string, index: number) => {
    const filename = `cropped-image-${index + 1}.png`
    ApiService.downloadImage(imageData, filename)
  }

  const loadConfigurations = async () => {
    try {
      configurations.value = await ApiService.getConfigurations()
    } catch (err: unknown) {
      error.value = getErrorMessage(err, 'Failed to load configurations')
      console.error('Load configurations error:', err)
    }
  }

  const createConfiguration = async (request: CreateConfigurationRequest) => {
    try {
      isLoading.value = true
      error.value = ''

      const newConfig = await ApiService.createConfiguration(request)
      configurations.value.push(newConfig)

      return newConfig
    } catch (err: unknown) {
      error.value = getErrorMessage(err, 'Failed to create configuration')
      console.error('Create configuration error:', err)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const setSelectedConfiguration = (configId: number | null) => {
    selectedConfig.value = configId
  }

  return {
    uploadedImage,
    imageUrl,
    cropAreas,
    previews,
    generatedImages,
    configurations,
    selectedConfig,
    isLoading,
    error,
    hasImage,
    hasCropAreas,
    canPreview,
    canGenerate,
    setImage,
    addCropArea,
    removeCropArea,
    updateCropArea,
    clearCropData,
    clearAll,
    generatePreviews,
    generateImages,
    downloadImage,
    loadConfigurations,
    createConfiguration,
    setSelectedConfiguration,
  }
})
