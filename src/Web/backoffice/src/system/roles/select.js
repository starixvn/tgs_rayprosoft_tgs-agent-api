import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useRoleList } from "./client"
import {
    useUserBrandId,
    useUserLicenseeId,
    useUserResellerId,
    useUserRoleType
} from "../../user/context"
import Select from "../../select/Select"
import { useTrans } from "../../i18n"

export const RoleSelect = ({
    resellerId,
    licenseeId,
    brandId,
    placeholder,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "选择权限组",
        en: "Select Role"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let userResellerId = useUserResellerId()
    resellerId = resellerId ?? userResellerId

    let userLicenseeId = useUserLicenseeId()
    licenseeId = licenseeId ?? userLicenseeId

    let userBrandId = useUserBrandId()
    brandId = brandId ?? userBrandId

    let { isLoading, data, error } = useRoleList({
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
