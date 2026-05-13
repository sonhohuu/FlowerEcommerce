import { defineConfig } from 'vite'
import react, { reactCompilerPreset } from '@vitejs/plugin-react'
import babel from '@rolldown/plugin-babel'

export default defineConfig({
  plugins: [
    react()
  ],
  server: {
    proxy: {
      '/api': {
        target: 'https://flowerecommerceapi20260512222055-a2hcb9cqcackeac4.southeastasia-01.azurewebsites.net',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})
