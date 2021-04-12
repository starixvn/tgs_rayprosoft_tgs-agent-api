import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Box,
    Button,
    Set,
    useModalState,
    Flex,
    Level
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code } from "./menu"
import {
    FormattedNumber,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import numberColumn, {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import { today } from "../../date/range"
import DateTimeRangeInput from "../../date/DateTimeRangeInput"
import { useApiLogList } from "./client"
import { LogDetailDialogDisclosure } from "../LogDetailDialog"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = ({}) => {
    let todayRange = today()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        from: todayRange.start,
        to: todayRange.end,
        trackingId: "",
        ip: "",
        path: "",
        method: ""
    })

    let { pageIndex, pageSize } = state
    let { data = {}, isLoading, mutate, error, deleteCache } = useApiLogList(
        toApiParams(state)
    )

    let columns = useColumns({ mutate, items: data?.items })

    return (
        <PageContent
            title={<Trans zh="API Logs" en="API Logs" />}
            actionBar={
                <SearchBar
                    start={state?.from}
                    end={state?.to}
                    trackingId={state?.trackingId}
                    ip={state?.ip}
                    path={state?.path}
                    method={state?.method}
                    search={({ start, end, ...data }) => {
                        let newState = {
                            pageIndex: 0,
                            from: start,
                            to: end,
                            ...data
                        }
                        if (
                            equals(newState, {
                                pageIndex,
                                from: state?.from,
                                to: state?.to,
                                trackingId: state?.trackingId,
                                ip: state?.ip,
                                path: state?.path,
                                method: state?.method
                            })
                        ) {
                            mutate()
                        }
                        setState(newState)
                    }}
                    isLoading={isLoading}
                />
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({
    start,
    end,
    trackingId,
    ip,
    path,
    method,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            start,
            end,
            trackingId,
            ip,
            path,
            method
        }
    })

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <DateTimeRangeInput startName="start" endName="end" />
                    {/* <InputField
                        name="trackingId"
                        showEmptyFieldValueAddonAfter
                        placeholder="Tracking Id"
                    /> */}
                    <InputField
                        name="ip"
                        showEmptyFieldValueAddonAfter
                        placeholder="IP"
                    />
                    <InputField
                        name="path"
                        showEmptyFieldValueAddonAfter
                        placeholder="Path"
                    />
                    <InputField
                        name="method"
                        showEmptyFieldValueAddonAfter
                        placeholder="Method"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [] } = {}) => {
    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "id",
                Header: <Trans zh="Id" en="Id" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            // textColumn({
            //     accessor: "trackingId",
            //     Header: <Trans zh="Tracking Id" en="Tracking Id" />,
            //     disableSortBy: true,
            //     Cell: ({ value }) => value
            // }),
            textColumn({
                accessor: "ip",
                Header: <Trans zh="Ip" en="Ip" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            textColumn({
                accessor: "path",
                Header: <Trans zh="Path" en="Path" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            textColumn({
                accessor: "method",
                Header: <Trans zh="Method" en="Method" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            textColumn({
                accessor: "prefix",
                Header: <Trans zh="Prefix" en="Prefix" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            textColumn({
                accessor: "serverName",
                Header: <Trans zh="Server Name" en="Server Name" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            textColumn({
                accessor: "responseCode",
                Header: <Trans zh="Response Code" en="Response Code" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            textColumn({
                accessor: "executionTimeMs",
                Header: <Trans zh="Execution Time Ms" en="Execution Time Ms" />,
                disableSortBy: true,
                Cell: ({ value }) => <FormattedNumber value={value} />
            }),
            dateTimeColumn({
                accessor: "recordedOn",
                Header: <Trans zh="Recorded On" en="Recorded On" />,
                disableSortBy: true
            }),
            textColumn({
                id: "actions",
                accessor: (row) => row,
                Header: <Trans zh="Actions" en="Actions" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    return (
                        <Set alignX="center">
                            <LogDetailDialogDisclosure record={value} />
                        </Set>
                    )
                }
            })
        ]

        return columns
    }, [JSON.stringify(items)])
}
