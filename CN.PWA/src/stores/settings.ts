import { SettingsHelper } from '@/modules/SettingsHelper'
import { defineStore } from 'pinia'

export const useSettingsStore = defineStore('settings', {
  state: () => ({
    settings: {} as Settings
  }),
  getters: {},
  actions: {
    saveSettings(): void {
      localStorage.setItem('settings', JSON.stringify(this.settings))
    },
    fetchSettings(): void {
      this.settings = (JSON.parse(localStorage.getItem('settings') as string) ??
        SettingsHelper.GetNewDefaultSettings()) as Settings
    }
  }
})
