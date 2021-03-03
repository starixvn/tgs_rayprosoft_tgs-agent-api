import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { Heading, Stack, Box, Button, Set, useModalState } from "bumbag"
import PageContent from "../../page/Content"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { FormattedNumber, Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import { Code } from "./menu"
import BrandBar from "../BrandBar"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import { yesterday } from "../../date/range"
import { useMarketReportList } from "./client"
import dateTimeColumn from "../../table/column/dateTime"
import subDays from "date-fns/subDays"
import { startOfNextDay } from "../../date/startOfNextDay"
import numberColumn from "../../table/column/numberColumn"
import startOfTomorrow from "date-fns/startOfTomorrow"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

export const Body = () => {
    let [brandId, setBrandId] = useState()

    return (
        <Set orientation="horizontal">
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <ReportContent brandId={brandId} />}
        </Set>
    )
}

const ReportContent = ({ brandId }) => {
    let dateRange = yesterday()
    let QueryMaxRangeDays = 30
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        pageSize: QueryMaxRangeDays,
        brandId,
        from: dateRange.start,
        to: dateRange.end
    })

    let { pageIndex, pageSize, from, to } = state
    let { data = {}, isLoading, mutate } = useMarketReportList({
        ...toApiParams(state),
        sortby: "reportDate",
        from,
        to: startOfNextDay(to)
    })

    let DataRetentionDays = 180
    let minDate = subDays(dateRange.start, DataRetentionDays)

    let columns = useColumns({ items: data?.items })
    let maxDate = startOfTomorrow()
    return (
        <PageContent
            actionBar={
                <SearchBar
                    start={from}
                    end={to}
                    minDate={minDate}
                    maxDate={maxDate}
                    maxRangeDays={QueryMaxRangeDays}
                    isLoading={isLoading}
                    search={({ start, end }) => {
                        let newState = {
                            pageIndex: 0,
                            from: start,
                            to: end
                        }

                        if (
                            equals(newState, {
                                pageIndex,
                                from: state?.from,
                                to: state?.to
                            })
                        ) {
                            mutate()
                        }
                        setState(newState)
                    }}
                />
            }
            title={<Trans zh="市场报表" en="Market Report" />}
            paddingTop="1em"
            wrapperProps={{
                width: "calc(100% - 1em)"
            }}
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
                foot={data?.items?.length > 0}
            />
        </PageContent>
    )
}

const SearchBar = ({
    start,
    end,
    minDate,
    maxDate,
    maxRangeDays,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            start,
            end
        }
    })

    const onSubmit = (data) => search?.(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <DateInputRange
                        minDate={minDate}
                        maxDate={maxDate}
                        maxRangeDays={maxRangeDays}
                        required
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [] }) => {
    return useMemo(
        () => [
            dateTimeColumn({
                Header: <Trans zh="日期" en="Date" />,
                accessor: "reportDate",
                dateFormat: "dd/MM/yyyy",
                timeFormat: false,
                disableSortBy: true,
                Footer: () => <Trans zh="共计" en="Total" />,
                footerCellProps: {
                    textAlign: "center"
                }
            }),
            numberColumn({
                accessor: "registeredCount",
                sumColumn: "registeredCount",
                Header: <Trans zh="注册人数" en="Registered Count" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "firstFundInCount",
                sumColumn: "firstFundInCount",
                Header: <Trans zh="首充人数" en="First Fund In Count" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "depositAmount",
                sumColumn: "depositAmount",
                Header: <Trans zh="存款金额" en="Deposit Amount" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "withdrawalAmount",
                sumColumn: "withdrawalAmount",
                Header: <Trans zh="提款金额" en="Withdrawal Amount" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "adjustmentAmount",
                sumColumn: "adjustmentAmount",
                Header: <Trans zh="调账" en="Adjustment Amount" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "bonusAmount",
                sumColumn: "bonusAmount",
                Header: <Trans zh="红利金额" en="Bonus Amount" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "betAmount",
                sumColumn: "betAmount",
                Header: <Trans zh="投注金额" en="Bet Amount" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "validBetAmount",
                sumColumn: "validBetAmount",
                Header: <Trans zh="有效投注" en="Valid Bet Amount" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "winLossAmount",
                sumColumn: "winLossAmount",
                Header: <Trans zh="输赢金额" en="Win Loss Amount" />,
                disableSortBy: true
            }),
            numberColumn({
                accessor: "netProfit",
                sumColumn: "netProfit",
                Header: <Trans zh="净盈利" en="NetProfit" />,
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
}
