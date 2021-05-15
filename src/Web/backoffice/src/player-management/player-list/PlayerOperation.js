import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Spinner, Stack, Flex, Box, Card, Set } from "bumbag"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import dateTimeColumn from "../../table/column/dateTime"
import { useOperations } from "./client"
import { Trans } from "../../i18n"
import { pastWeek } from "../../date/range"
import DateTimeRangeInput from "../../date/DateTimeRangeInput"
import SearchButton from "../../button/SearchButton"
import { Tab as _Tab, TabPanel } from "../../tab"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import isSameDate from "date-fns/isEqual"
import textColumn from "../../table/column/textColumn"
import { Descending } from "../../table/order"
import AlertDialog from "../../dialog/Alert"

export const Key = "operation"

export const PlayerOperationTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="操作记录" en="Operating records" />
        </_Tab>
    )
}

export const PlayerOperationPanel = ({ playerCode, ...props }) => {
    let { selectedId } = props
    let isActive = selectedId === Key

    let dateRange = pastWeek()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        from: dateRange.start,
        to: dateRange.end
    })

    let { pageSize, from, to } = state
    let { isLoading, data, error, mutate, deleteCache } = useOperations(
        isActive ? playerCode : undefined,
        toApiParams({ ...state, sortby: "createdOn", orderby: Descending })
    )

    let columns = useColumns({ items: data?.items })
    return (
        <TabPanel tabId={Key} {...props}>
            <Stack spacing="major-2">
                <SearchBar
                    start={from}
                    end={to}
                    search={({ start, end }) => {
                        if (isSameDate(from, start) && isSameDate(to, end)) {
                            mutate()
                        }
                        setState({
                            from: start,
                            to: end
                        })
                    }}
                    isLoading={isLoading}
                />
                <Table
                    columns={columns}
                    data={data?.items}
                    pageCount={pageCount(pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    initialState={state}
                    isLoading={isLoading}
                />
            </Stack>
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </TabPanel>
    )
}

const SearchBar = ({ start, end, search, isLoading, ...props }) => {
    let formMethods = useForm({
        defaultValues: {
            start,
            end
        }
    })

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <DateTimeRangeInput />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [] }) =>
    useMemo(
        () => [
            dateTimeColumn({
                Header: <Trans zh="操作时间" en="Operation Time" />,
                accessor: "createdOn",
                disableSortBy: true
            }),
            textColumn({
                accessor: "operation",
                Header: <Trans zh="操作项" en="Operation Item" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "createdByUsername",
                Header: <Trans zh="操作者" en="Operator" />,
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
