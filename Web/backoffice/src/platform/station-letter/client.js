import useSWR, { cache } from "swr"
import client, {
  urlWith,
  get,
  put,
  post,
  useGet,
  usePostData,
  usePutData,
  usePostFormData
} from "../../client"

const API = "/api/stationletter"

export const useStationLetterList = (searchParams = {}) => {
  let url = urlWith(searchParams, API)
  return useGet(url)
}

export const useVerifyStationLetter = ({ id, type, data } = {}) => {
  let url = `${API}/${id}/${type}`
  return usePutData(url, data)
}

export const useAddStationLetter = (data) => {
  return usePostData(API, data)
}

export const useStationLetterUpload = (body) => {
  return usePostFormData(API, body)
}
