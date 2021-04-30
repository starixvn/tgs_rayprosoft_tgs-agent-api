import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { CheckboxGroupField } from "bumbag"
import AlertDialog from "../dialog/Alert"
import { useVipLevelList } from "./client"
import { CheckboxGroup } from "../checkbox/CheckboxGroup"

export const VipLevelCheckboxGroup = ({
    orientation = "horizontal",
    valueFieldName = "id",
    brandId,
    ...props
}) => {
    let { isLoading, data, error } = useVipLevelList({ brandId, showAll: true })

    let options =
        data?.items?.map((item) => ({
            label: item.name,
            value: `${Reflect.get(item, valueFieldName)}`
        })) ?? []

    return (
        <CheckboxGroup
            isLoading={isLoading}
            options={options}
            orientation={orientation}
            {...props}
        />
    )
}
