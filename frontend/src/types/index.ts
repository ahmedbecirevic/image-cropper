export interface CropCoordinates {
  x: number
  y: number
  width: number
  height: number
}

export interface ImageCropRequest {
  image: File
  cropCoordinates: CropCoordinates[]
}

export interface ImageCropResponse {
  coordinates: CropCoordinates
  imageData: string // base64 encoded
  contentType: string
  hasLogoOverlay?: boolean
  error?: string
}

export interface Configuration {
  id: number
  scaleDown: number
  logoPosition: string
  hasLogoImage: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateConfigurationRequest {
  scaleDown: number
  logoPosition: string
  logoImage?: File
}

export interface CropArea {
  x: number
  y: number
  width: number
  height: number
  id: string
}

export type LogoPosition = 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right' | 'center'
