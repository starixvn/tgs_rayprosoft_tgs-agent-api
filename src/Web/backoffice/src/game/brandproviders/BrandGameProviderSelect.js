import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { Heading, Stack, Button, InputField, Set, Text, Box } from "bumbag"
import { Select } from "../../select/Select"
import { useBrandGameProviderList } from "./client"
import { useTrans } from "../../i18n"
import { useUserBrandId } from "../../user/context"

export const BrandGameProviderSelect = ({ brandId, placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "厂商",
        en: "Select Game Provider"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let userBrandId = useUserBrandId()
    brandId = userBrandId ?? brandId
    let { data, isLoading } = useBrandGameProviderList({
        showAll: true,
        brandId
    })

    let options =
        data?.items?.map((gameProvider) => ({
            label: gameProvider.gameProviderName,
            value: gameProvider.gameProviderId
        })) ?? []

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}

export default BrandGameProviderSelect
