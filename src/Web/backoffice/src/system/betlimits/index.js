import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Icon } from "bumbag"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserRoleType,
    useUserRoleId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Code } from "./menu"
import textColumn from "../../table/column/textColumn"
import dateTimeColumn from "../../table/column/dateTime"
import { useForm, FormProvider } from "react-hook-form"
import { InputField } from "../../input/Input"
import SearchButton from "../../button/SearchButton"
import { useBetLimitList } from "../../betlimit/client"
import {
    BetLimitTrans,
    BetLimitMinBetTrans,
    BetLimitMaxBetTrans,
    BetLimitMaxBetPerMatchTrans
} from "../../betlimit/trans"
import { CurrencyTrans } from "../../currency/trans"
import { PageContent } from "../../page/Content"

export const SystemBetLimitsCode = Code

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [SystemBetLimitsCode, Content]

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        keyword: ""
    })

    let { data, isLoading, mutate } = useBetLimitList(toApiParams(state))

    let columns = useColumns({ mutate, items: data?.items })
    return (
        <PageContent title={<BetLimitTrans />}>
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
    return useMemo(
        () => [
            textColumn({
                accessor: "currency",
                Header: <CurrencyTrans />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "minBet",
                Header: <BetLimitMinBetTrans />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "maxBet",
                Header: <BetLimitMaxBetTrans />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "maxBetPerMatch",
                Header: <BetLimitMaxBetPerMatchTrans />,
                disableSortBy: true
            }),
            dateTimeColumn({
                Header: <Trans zh="创建时间" en="Created Time" />,
                accessor: "createdOn",
                disableSortBy: true
            }),
            dateTimeColumn({
                Header: <Trans zh="更改时间" en="Updated Time" />,
                accessor: "updatedOn",
                disableSortBy: true
            }),
            textColumn({
                accessor: "createdByUsername",
                Header: <Trans zh="创建人" en="Created By" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "UpdatedByUsername",
                Header: <Trans zh="更新人" en="Updated By" />,
                disableSortBy: true
            }),
            {
                id: "actions",
                accessor: (row) => row,
                Cell: ({ value }) => {
                    return (
                        <Set orientation="horizontal">
                            TODO: edit & delete btn
                        </Set>
                    )
                },
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                }
            }
        ],
        [JSON.stringify(items)]
    )
}
