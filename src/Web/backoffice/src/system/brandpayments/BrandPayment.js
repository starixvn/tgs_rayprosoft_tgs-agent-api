import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Box,
    Button,
    Set,
    Flex,
    Level,
    FieldStack
} from "bumbag"
import { useBrandPaymentList } from "./client"
import { Select, SelectField } from "../../select/Select"

export const BrandPaymentSelect = ({
    brandId,
    transferType,
    paymentChannelId,
    ...props
}) => {
    let { isLoading, data, error, deleteCache, mutate } = useBrandPaymentList({
        brandId,
        transferType,
        sortby: "number",
        showAll: true
    })

    let options = data?.items
        ?.filter((brandPayment) => {
            if (
                paymentChannelId &&
                paymentChannelId != brandPayment.paymentChannelId
            ) {
                return false
            }

            return true
        })
        ?.map((item) => ({
            label: item.channelName,
            value: item.id
        }))
    return <Select isLoading={isLoading} options={options} {...props} />
}
