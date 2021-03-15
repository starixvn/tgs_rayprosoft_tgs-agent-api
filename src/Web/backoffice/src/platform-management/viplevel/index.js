import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Flex, Set, Button, Text, Stack } from "bumbag"
import PageContent from "../../page/Content"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId,
    useUserRoleType
} from "../../user/context"
import { View as ViewMenu } from "./menu"
import { useFormContext, useForm, FormProvider } from "react-hook-form"
import { Code } from "./menu"
import BrandBar from "../../report/BrandBar"
import { toApiParams, useTable, Table, pageCount } from "../../table"
import { ViewButton, EditButton, AddButton } from "./edit"
import { Trans } from "../../i18n"
import textColumn from "../../table/column/textColumn"
import { useVipLevelList } from "../../vip-levels/client"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId }) => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        brandId
    })

    useEffect(() => {
        setState({ brandId })
    }, [brandId])

    let { data, isLoading, mutate } = useVipLevelList(toApiParams(state))

    let columns = useColumns({ items: data?.items, mutate })
    return (
        <PageContent
            actionBar={
                <Box alignX="right">
                    <AddButton onSubmitted={mutate} />
                </Box>
            }
            title={<Trans zh="VIP等级" en="VIP Level" />}
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(state?.pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
        </PageContent>
    )
}

const useColumns = ({ items = [], mutate }) => {
    let hasAnyMenu = useHasSubMenuByRoute()

    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "name",
                Header: <Trans zh="等级名称" en="VIP Level" />,
                disableSortBy: true
            })
        ]

        if (hasAnyMenu) {
            columns.push({
                id: "actions",
                accessor: (row) => row,
                disableSortBy: true,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    return (
                        <Set alignX="center">
                            <ViewButton record={value} />
                            <EditButton record={value} onSubmitted={mutate} />
                        </Set>
                    )
                }
            })
        }

        return columns
    }, [JSON.stringify(items)])
}
