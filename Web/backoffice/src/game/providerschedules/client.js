import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../client"

const API = "/api/brandgameproviderschedule"

export const useGameProviderScheduleList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useUpdateGameProviderSchedule = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useGameProviderSchedulePresetMaintenanceList = (id) => {
  let url = id ? `${API}/${id}/defaultTimes` : ""
  return useGet(url)
}

export const useUpdateGameProviderSchedulePresetMaintenance = (id, data) => {
  let url = id ? `${API}/${id}/defaultTimes` : ""
  return usePutData(url, data)
}

export const useChangeGameProviderScheduleStatus = (id, status) => {
  let url = id && status ? `${API}/${id}/${status}` : ""
  return usePut(url)
}
