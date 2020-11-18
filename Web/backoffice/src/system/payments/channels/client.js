import useSWR, { cache } from "swr"
import { urlWith, useGet, usePostData, usePutData } from "../../../client"

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
  return usePostData(PaymentChannelAPI, data)
}

export const useUpdatePaymentChannel = (id, data) => {
  let url = id ? `${PaymentChannelAPI}/${id}` : ""
  return usePutData(url, data)
}
