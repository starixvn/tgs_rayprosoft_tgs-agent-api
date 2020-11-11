import useSWR, { cache } from "swr"
import { urlWith, useGet, useCreatePost, useUpdatePut } from "../../../client"

const PaymentChannelAPI = "/api/paymentchannel"

export const usePaymentChannelList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, PaymentChannelAPI) : ""
  return useGet(url)
}

export const usePaymentChannel = (id) => {
  let url = id ? `${PaymentChannelAPI}/${id}` : ""
  return useGet(url)
}

export const useAddPaymentChannel = (data) => {
  return useCreatePost(PaymentChannelAPI, data)
}

export const useUpdatePaymentChannel = (id, data) => {
  let url = id ? `${PaymentChannelAPI}/${id}` : ""
  return useUpdatePut(url, data)
}
