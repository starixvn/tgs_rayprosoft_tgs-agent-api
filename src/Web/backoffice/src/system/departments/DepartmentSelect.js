import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useDepartmentList } from "./client"
import { Select } from "../../select/Select"
import { useTrans } from "../../i18n"

export const DepartmentSelect = ({
    placeholder,
    resellerId,
    licenseeId,
    brandId,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "选择部门",
        en: "Select Departmen"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { isLoading, data, error } = useDepartmentList({
        showAll: true,
        resellerId,
        licenseeId,
        brandId
    })

    let options = toOptions(
        { resellerId, licenseeId, brandId },
        data?.items
    ).map(({ id, name }) => ({
        label: name,
        value: id
    }))

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}

const toOptions = ({ resellerId, licenseeId, brandId }, items = []) =>
    items.filter((item) => {
        if (licenseeId && !brandId) {
            return item.licenseeId && !item.brandId
        }

        if (resellerId && !licenseeId) {
            return item.resellerId && !item.licenseeId
        }

        if (!resellerId) {
            return !item.resellerId
        }

        return true
    })
