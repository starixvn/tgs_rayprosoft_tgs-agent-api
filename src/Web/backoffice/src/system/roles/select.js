import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { SelectField, Dialog, Modal, Table, useClassName } from "bumbag"
import AlertDialog from "../../dialog/Alert"
import { useRoleList } from "./client"
import {
    useUserBrandId,
    useUserLicenseeId,
    useUserResellerId,
    useUserRoleType
} from "../../user/context"
import Select from "../../select/Select"
import { useTrans } from "../../i18n"
import { Controller, useFormContext } from "react-hook-form"

export const RoleSelect = ({
    resellerId,
    licenseeId,
    brandId,
    placeholder,
    placeholderDisabled,
    ...props
}) => {
    let formMethods = useFormContext()
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

    let { error, data = {}, isLoading } = useRoleList({
        showAll: true,
        resellerId,
        licenseeId,
        brandId
    })

    if (
        isLoading &&
        props?.name &&
        props?.defaultValue &&
        formMethods?.control
    ) {
        return (
            <Controller
                name={props?.name}
                control={formMethods?.control}
                render={() => <Select isLoading={isLoading} options={[]} />}
                defaultValue={props?.defaultValue}
            />
        )
    }

    let roleOptions =
        toOptions({ resellerId, licenseeId, brandId }, data?.items).map(
            ({ id, name }) => ({
                label: name,
                value: id
            })
        ) ?? []

    let options =
        typeof placeholder === "string"
            ? [
                  {
                      label: placeholder,
                      value: "",
                      disabled: placeholderDisabled
                  },
                  ...roleOptions
              ]
            : roleOptions

    return (
        <>
            <Select options={options} {...props} />
            {error && <AlertDialog {...error} />}
        </>
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
