-- In this file we will document the database schema for our application.

-- Table: email_whitelist
create table email_whitelist(
  id uuid primary key default gen_random_uuid(),
  email text unique not null,
  role text not null check (role in ('Admin', 'Ranger', 'Scientist'))
);

-- RLS policies email_whitelist
create policy "admin_full_access"
on email_whitelist
for all
using(auth.jwt() ->> 'role' = 'Admin')
with check (auth.jwt() ->> 'role' = 'Admin')
