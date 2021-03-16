import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import AlertDialog from "../../dialog/Alert"
import { useDepartmentList } from "./client"
import { Select } from "../../select/Select"
import { Controller, useFormContext } from "react-hook-form"
import { useTrans } from "../../i18n"

export const DepartmentSelect = ({
    resellerId,
    licenseeId,
    brandId,
    placeholder,
    placeholderDisabled,
    ...props
}) => {
    let formMethods = useFormContext()
    let defaultPlaceholder = useTrans({
        zh: "选择部门",
        en: "Select Departmen"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { error, data = {}, isLoading } = useDepartmentList({
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

    let departmentOptions = toOptions(
        { resellerId, licenseeId, brandId },
        data?.items
    ).map(({ id, name }) => ({
        label: name,
        value: id
    }))

    let options =
        typeof placeholder === "string"
            ? [
                  {
                      label: placeholder,
                      value: "",
                      disabled: placeholderDisabled
                  },
                  ...departmentOptions
              ]
            : departmentOptions

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
