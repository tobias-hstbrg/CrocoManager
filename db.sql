CREATE TABLE public.animals (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    name character varying(255) NOT NULL,
    species character varying(255) NOT NULL,
    gender character varying(50),
    age_years integer DEFAULT 0,
    enclosure character varying(255),
    description text,
    created_at timestamp with time zone DEFAULT now(),
    updated_at timestamp with time zone DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE public.email_whitelist (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    email text NOT NULL,
    role text NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public.environmental_data (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    measurement_date date NOT NULL,
    measurement_time time without time zone NOT NULL,
    air_temperature_celsius numeric(5,2) NOT NULL,
    humidity_percent numeric(5,2) NOT NULL,
    water_temperature_celsius numeric(5,2) NOT NULL,
    ph_value numeric(4,2) NOT NULL,
    salinity_ppt numeric(5,2) NOT NULL,
    created_at timestamp with time zone DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE public.feeding_plan (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    name character varying(255) NOT NULL,
    food_type character varying(255) NOT NULL,
    amount_kg numeric(10,2) NOT NULL,
    frequency_per_week integer NOT NULL,
    weekdays text[],
    description text,
    is_active boolean DEFAULT true,
    created_at timestamp with time zone DEFAULT now(),
    updated_at timestamp with time zone DEFAULT now(),
    PRIMARY KEY (id)
);

CREATE TABLE public.feedings (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    feeding_plan_id uuid NOT NULL,
    feeding_date date NOT NULL,
    performed_by_email character varying(255) NOT NULL,
    created_at timestamp with time zone DEFAULT now(),
    updated_at timestamp with time zone DEFAULT now(),
    PRIMARY KEY (id),
    FOREIGN KEY (feeding_plan_id) REFERENCES public.feeding_plan (id)
);

CREATE TABLE public.feeding_animals (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    feeding_id uuid NOT NULL,
    animal_id uuid NOT NULL,
    was_fed boolean DEFAULT true,
    created_at timestamp with time zone DEFAULT now(),
    PRIMARY KEY (id),
    FOREIGN KEY (animal_id) REFERENCES public.animals (id),
    FOREIGN KEY (feeding_id) REFERENCES public.feedings (id)
);

CREATE TABLE public.observations (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    animal_id uuid NOT NULL,
    feeding_id uuid NOT NULL,
    environmental_data_id uuid,
    feeding_behavior character varying(255),
    notes text,
    researcher_email character varying(255) NOT NULL,
    created_at timestamp with time zone DEFAULT now(),
    updated_at timestamp with time zone DEFAULT now(),
    PRIMARY KEY (id),
    FOREIGN KEY (animal_id) REFERENCES public.animals (id),
    FOREIGN KEY (environmental_data_id) REFERENCES public.environmental_data (id),
    FOREIGN KEY (feeding_id) REFERENCES public.feedings (id)
);