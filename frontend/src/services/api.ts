import axios, { type AxiosResponse } from 'axios'
import type {
  ImageCropRequest,
  ImageCropResponse,
  Configuration,
  CreateConfigurationRequest,
} from '@/types'
import { useAuthStore } from '@/stores/auth'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'multipart/form-data',
  },
})

// Add request interceptor to include auth token
apiClient.interceptors.request.use(
  async (config) => {
    const authStore = useAuthStore()
    const token = await authStore.getAccessToken()

    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

export class ApiService {
  static async previewImage(request: ImageCropRequest): Promise<ImageCropResponse[]> {
    const formData = new FormData()
    formData.append('image', request.image)

    request.cropCoordinates.forEach((coord, index) => {
      formData.append(`cropCoordinates[${index}].x`, coord.x.toString())
      formData.append(`cropCoordinates[${index}].y`, coord.y.toString())
      formData.append(`cropCoordinates[${index}].width`, coord.width.toString())
      formData.append(`cropCoordinates[${index}].height`, coord.height.toString())
    })

    const response: AxiosResponse<ImageCropResponse[]> = await apiClient.post(
      '/image/preview',
      formData,
    )
    return response.data
  }

  static async generateImage(
    request: ImageCropRequest,
    configId?: number,
  ): Promise<ImageCropResponse[]> {
    const formData = new FormData()
    formData.append('image', request.image)

    request.cropCoordinates.forEach((coord, index) => {
      formData.append(`cropCoordinates[${index}].x`, coord.x.toString())
      formData.append(`cropCoordinates[${index}].y`, coord.y.toString())
      formData.append(`cropCoordinates[${index}].width`, coord.width.toString())
      formData.append(`cropCoordinates[${index}].height`, coord.height.toString())
    })

    const url = configId ? `/image/generate?configId=${configId}` : '/image/generate'
    const response: AxiosResponse<ImageCropResponse[]> = await apiClient.post(url, formData)
    return response.data
  }

  static async getConfigurations(): Promise<Configuration[]> {
    const response: AxiosResponse<Configuration[]> = await apiClient.get('/config', {
      headers: { 'Content-Type': 'application/json' },
    })
    return response.data
  }

  static async createConfiguration(request: CreateConfigurationRequest): Promise<Configuration> {
    const formData = new FormData()
    formData.append('scaleDown', request.scaleDown.toString())
    formData.append('logoPosition', request.logoPosition)

    if (request.logoImage) {
      formData.append('logoImage', request.logoImage)
    }

    const response: AxiosResponse<Configuration> = await apiClient.post('/config', formData)
    return response.data
  }

  static downloadImage(base64Data: string, filename: string = 'cropped-image.png') {
    const link = document.createElement('a')
    link.href = `data:image/png;base64,${base64Data}`
    link.download = filename
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  }
}
